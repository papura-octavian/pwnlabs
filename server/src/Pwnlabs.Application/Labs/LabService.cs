using Microsoft.Extensions.Options;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Labs.Dtos;
using Pwnlabs.Domain.Entities;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Domain.Enums;

namespace Pwnlabs.Application.Labs;

public class LabService : ILabService
{
    private readonly ILabRepository _labRepository;
    private readonly ILabOrchestrator _orchestrator;
    private readonly IRoomRepository _roomRepository;
    private readonly LabOptions _options;

    public LabService(
        ILabRepository labRepository,
        ILabOrchestrator orchestrator,
        IRoomRepository roomRepository,
        IOptions<LabOptions> options)
    {
        _labRepository = labRepository;
        _orchestrator = orchestrator;
        _roomRepository = roomRepository;
        _options = options.Value;
    }

    public async Task<LabResponse> StartAsync(Guid userId, string roomSlug)
    {
        var count = await _labRepository.CountActiveByUserAsync(userId);
        if (count >= _options.MaxActiveLabsPerUser)
        {
            throw new ConflictException("You already have an active lab. Stop it before starting a new one.");
        }

        var room = await _roomRepository.GetPublishedBySlugAsync(roomSlug);

        if (room is null)
        {
            throw new NotFoundException("Room not found.");
        }

        var labId = Guid.CreateVersion7();

        var started = await _orchestrator.StartLabAsync(room.Image, room.ContainerPort, labId, userId);

        var lab = new LabInstance
        {
            Id = labId,
            UserId = userId,
            ContainerId = started.ContainerId,
            Image = room.Image,
            HostPort = started.HostPort,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_options.LifetimeMinutes),
            RoomId = room.Id
        };

        _labRepository.Add(lab);
        await _labRepository.SaveChangesAsync();

        return ToResponse(lab);
    }

    public async Task<List<LabResponse>> GetMyLabsAsync(Guid userId)
    {
        var labs = await _labRepository.GetActiveByUserAsync(userId);

        return labs.Select(l => ToResponse(l)).ToList();
    }

    public async Task StopAsync(Guid userId, Guid labId)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        if (lab is null || lab.UserId != userId)
        {
            throw new NotFoundException("Lab not found.");
        }

        if (lab.Status != LabStatus.Running)
        {
            return;
        }

        await _orchestrator.StopLabAsync(lab.ContainerId);
        lab.Status = LabStatus.Stopped;
        lab.StoppedAt = DateTime.UtcNow;
        await _labRepository.SaveChangesAsync();
    }

    private static LabResponse ToResponse(LabInstance lab)
    {
        return new LabResponse
        {
            Id = lab.Id,
            Image = lab.Image,
            Url = $"http://127.0.0.1:{lab.HostPort}",
            Status = lab.Status.ToString(),
            CreatedAt = lab.CreatedAt,
            ExpiresAt = lab.ExpiresAt
        };
    }

    public async Task<int> ExpireLabsAsync()
    {
        var expired = await _labRepository.GetExpiredRunningAsync(DateTime.UtcNow);

        if (expired.Count == 0)
        {
            return 0;
        }

        int stoppedCount = 0;
        foreach (var lab in expired)
        {
            try
            {
                await _orchestrator.StopLabAsync(lab.ContainerId);
                lab.Status = LabStatus.Stopped;
                lab.StoppedAt = DateTime.UtcNow;
                stoppedCount++;
            }
            catch (Exception)
            {

            }
        }

        if (stoppedCount > 0)
        {
            await _labRepository.SaveChangesAsync();
        }

        return stoppedCount;
    }

    public async Task ReconcileAsync()
    {
        var labs = await _labRepository.GetAllRunningAsync();
        var containers = await _orchestrator.ListManagedContainersAsync();

        var failedCount = 0;
        var containerIds = containers.Select(c => c.ContainerId).ToHashSet();
        foreach (var lab in labs)
        {

            if (!containerIds.Contains(lab.ContainerId))
            {
                lab.Status = LabStatus.Failed;
                lab.StoppedAt = DateTime.UtcNow;
                failedCount++;
            }
        }

        if (failedCount > 0)
        {
            await _labRepository.SaveChangesAsync();
        }

        var runningLabIds = labs.Select(l => l.Id).ToHashSet();
        foreach (var container in containers)
        {
            if (container.LabId is null || !runningLabIds.Contains(container.LabId.Value))
            {
                await _orchestrator.StopLabAsync(container.ContainerId);
            }
        }

    }
}