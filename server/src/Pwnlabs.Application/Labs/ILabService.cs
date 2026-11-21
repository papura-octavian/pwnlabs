using Pwnlabs.Application.Labs.Dtos;

namespace Pwnlabs.Application.Labs;

public interface ILabService
{
    Task<LabResponse> StartAsync(Guid userId, string roomSlug);
    Task<List<LabResponse>> GetMyLabsAsync(Guid userId);
    Task StopAsync(Guid userId, Guid labId);
    Task<int> ExpireLabsAsync();
    Task ReconcileAsync();
}