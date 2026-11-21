namespace Pwnlabs.Application.Abstractions;

public interface ILabOrchestrator
{
    Task<StartedLab> StartLabAsync(string image, int containerPort, Guid labId, Guid userId);
    Task StopLabAsync(string containerId);
    Task<List<ManagedContainer>> ListManagedContainersAsync();
}