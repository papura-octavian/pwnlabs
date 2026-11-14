using Pwnlabs.Application.Labs.Dtos;

namespace Pwnlabs.Application.Labs;

public interface ILabService
{
    Task<LabResponse> StartAsync(Guid userId);
    Task<List<LabResponse>> GetMyLabsAsync(Guid userId);
    Task StopAsync(Guid userId, Guid labId);
}