using Microsoft.Extensions.Options;
using NSubstitute;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Application.Labs;
using Pwnlabs.Domain.Entities;

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
}