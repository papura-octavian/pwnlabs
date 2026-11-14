using Microsoft.Extensions.Options;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Labs.Dtos;
using Pwnlabs.Domain.Entities;
using Pwnlabs.Application.Common.Exceptions;

namespace Pwnlabs.Application.Labs;

public class LabService : ILabService
{
    private readonly ILabRepository _labRepository;
    private readonly ILabOrchestrator _orchestrator;
    private readonly LabOptions _options;

    public LabService(
        ILabRepository labRepository,
        ILabOrchestrator orchestrator,
        IOptions<LabOptions> options)
    {
        _labRepository = labRepository;
        _orchestrator = orchestrator;
        _options = options.Value;
    }

    public async Task<LabResponse> StartAsync(Guid userId)
    {
        var count = await _labRepository.CountActiveByUserAsync(userId);
        if (count >= _options.MaxActiveLabsPerUser)
        {
            throw new ConflictException("You already have an active lab. Stop it before starting a new one.");
        }

        var labId = Guid.CreateVersion7();

        var started = await _orchestrator.StartLabAsync(_options.DefaultImage, labId, userId);

        var lab = new LabInstance
        {
            Id = labId,
            UserId = userId,
            ContainerId = started.ContainerId,
            Image = _options.DefaultImage,
            HostPort = started.HostPort,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_options.LifetimeMinutes)
        };

        _labRepository.Add(lab);
        await _labRepository.SaveChangesAsync();

        return ToResponse(lab);
    }

    public Task<List<LabResponse>> GetMyLabsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(Guid userId, Guid labId)
    {
        throw new NotImplementedException();
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
}