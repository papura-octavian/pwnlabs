namespace Pwnlabs.Application.Abstractions;

public interface ILabOrchestrator
{
    Task<StartedLab> StartLabAsync(string image, Guid labId, Guid userId);
    Task StopLabAsync(string containerId);
    Task<List<ManagedContainer>> ListManagedContainersAsync();
}