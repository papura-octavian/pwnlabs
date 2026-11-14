using Microsoft.Extensions.Options;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Labs.Dtos;

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

    public Task<LabResponse> StartAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<LabResponse>> GetMyLabsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(Guid userId, Guid labId)
    {
        throw new NotImplementedException();
    }
}