using Microsoft.Extensions.Options;
using NSubstitute;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Application.Labs;
using Pwnlabs.Domain.Entities;
using Pwnlabs.Domain.Enums;

namespace Pwnlabs.Application.Tests.Labs;

public class LabServiceTests
{
    private readonly ILabRepository _labRepository = Substitute.For<ILabRepository>();
    private readonly ILabOrchestrator _orchestrator = Substitute.For<ILabOrchestrator>();
    private readonly LabService _sut;

    private readonly Guid _userId = Guid.NewGuid();

    public LabServiceTests()
    {
        var options = Options.Create(new LabOptions
        {
            DefaultImage = "nginx:alpine",
            LifetimeMinutes = 60,
            MaxActiveLabsPerUser = 1
        });

        _sut = new LabService(_labRepository, _orchestrator, options);
    }

    [Fact]
    public async Task StartAsync_NoActiveLabs_StartsContainerAndSavesLab()
    {
        // Arrange
        _labRepository.CountActiveByUserAsync(_userId).Returns(0);
        _orchestrator.StartLabAsync(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns(new StartedLab
            {
                ContainerId = "container-123",
                HostPort = 55012
            });

        // Act
        var before = DateTime.UtcNow;
        var result = await _sut.StartAsync(_userId);
        var after = DateTime.UtcNow;

        // Assert
        await _orchestrator.Received(1).StartLabAsync("nginx:alpine", Arg.Any<Guid>(), _userId);

        _labRepository.Received(1).Add(Arg.Is<LabInstance>(l =>
            l.UserId == _userId &&
            l.ContainerId == "container-123" &&
            l.HostPort == 55012));

        await _labRepository.Received(1).SaveChangesAsync();

        Assert.Equal("http://127.0.0.1:55012", result.Url);
        Assert.InRange(result.ExpiresAt, before.AddMinutes(60), after.AddMinutes(60));
    }

    [Fact]
    public async Task StartAsync_ActiveLabExists_ThrowsConflictException()
    {
        // Arrange
        _labRepository.CountActiveByUserAsync(_userId).Returns(1);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() => _sut.StartAsync(_userId));
        await _orchestrator.DidNotReceive().StartLabAsync(
            Arg.Any<string>(), 
            Arg.Any<Guid>(), 
            Arg.Any<Guid>()
        );
    }

    [Fact]
    public async Task StopAsync_LabOfAnotherUser_ThrowsNotFoundException()
    {
        // Arrange
        var lab = CreateLab(ownerId: Guid.NewGuid());
        _labRepository.GetByIdAsync(lab.Id).Returns(lab);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.StopAsync(_userId, lab.Id));
        await _orchestrator.DidNotReceive().StopLabAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task StopAsync_LabDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var labId = Guid.NewGuid();
        _labRepository.GetByIdAsync(labId).Returns((LabInstance?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.StopAsync(_userId, labId));
    }

    [Fact]
    public async Task StopAsync_OwnRunningLab_StopsContainerAndMarksStopped()
    {
        // Arrange
        var lab = CreateLab(ownerId: _userId);
        _labRepository.GetByIdAsync(lab.Id).Returns(lab);

        // Act
        await _sut.StopAsync(_userId, lab.Id);

        // Assert
        await _orchestrator.Received(1).StopLabAsync("container-123");
        Assert.Equal(LabStatus.Stopped, lab.Status);
        Assert.NotNull(lab.StoppedAt);
        await _labRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task StopAsync_AlreadyStoppedLab_DoesNothing()
    {
        // Arrange
        var lab = CreateLab(ownerId: _userId, status: LabStatus.Stopped);
        _labRepository.GetByIdAsync(lab.Id).Returns(lab);

        // Act
        await _sut.StopAsync(_userId, lab.Id);

        // Assert
        await _orchestrator.DidNotReceive().StopLabAsync(Arg.Any<string>());
        await _labRepository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task GetMyLabsAsync_ReturnsActiveLabsOfUser()
    {
        // Arrange
        var labs = new List<LabInstance> { CreateLab(_userId), CreateLab(_userId) };
        _labRepository.GetActiveByUserAsync(_userId).Returns(labs);

        // Act
        var result = await _sut.GetMyLabsAsync(_userId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal("http://127.0.0.1:55012", r.Url));
    }

    private static LabInstance CreateLab(Guid ownerId, LabStatus status = LabStatus.Running)
    {
        return new LabInstance
        {
            UserId = ownerId,
            ContainerId = "container-123",
            Image = "nginx:alpine",
            HostPort = 55012,
            Status = status,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    [Fact]
    public async Task ExpireLabsAsync_ExpiredLabs_StopsContainersAndMarksStopped()
    {
        // Arrange
        var lab1 = CreateLab(_userId);
        var lab2 = CreateLab(Guid.NewGuid());
        _labRepository.GetExpiredRunningAsync(Arg.Any<DateTime>())
            .Returns(new List<LabInstance> { lab1, lab2 });

        // Act
        var count = await _sut.ExpireLabsAsync();

        // Assert
        Assert.Equal(2, count);
        await _orchestrator.Received(2).StopLabAsync("container-123");
        Assert.Equal(LabStatus.Stopped, lab1.Status);
        Assert.Equal(LabStatus.Stopped, lab2.Status);
        Assert.NotNull(lab1.StoppedAt);
        await _labRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task ExpireLabsAsync_NoExpiredLabs_DoesNothing()
    {
        // Arrange
        _labRepository.GetExpiredRunningAsync(Arg.Any<DateTime>())
            .Returns(new List<LabInstance>());

        // Act
        var count = await _sut.ExpireLabsAsync();

        // Assert
        Assert.Equal(0, count);
        await _orchestrator.DidNotReceive().StopLabAsync(Arg.Any<string>());
        await _labRepository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task ExpireLabsAsync_OneContainerFailsToStop_StillStopsTheOthers()
    {
        // Arrange
        var failing = CreateLab(_userId);
        failing.ContainerId = "broken-container";
        var ok = CreateLab(Guid.NewGuid());

        _labRepository.GetExpiredRunningAsync(Arg.Any<DateTime>())
            .Returns(new List<LabInstance> { failing, ok });
        _orchestrator.StopLabAsync("broken-container")
            .Returns(Task.FromException(new Exception("Docker error")));

        // Act
        var count = await _sut.ExpireLabsAsync();

        // Assert
        Assert.Equal(1, count);
        Assert.Equal(LabStatus.Running, failing.Status);
        Assert.Equal(LabStatus.Stopped, ok.Status);
        await _labRepository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task ReconcileAsync_RunningLabWithoutContainer_IsMarkedFailed()
    {
        // Arrange
        var lab = CreateLab(_userId);
        _labRepository.GetAllRunningAsync().Returns(new List<LabInstance> { lab });
        _orchestrator.ListManagedContainersAsync().Returns(new List<ManagedContainer>());

        // Act
        await _sut.ReconcileAsync();

        // Assert
        Assert.Equal(LabStatus.Failed, lab.Status);
        Assert.NotNull(lab.StoppedAt);
        await _labRepository.Received(1).SaveChangesAsync();
    }

    
}