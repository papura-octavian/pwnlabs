namespace Pwnlabs.Application.Abstractions;

public record ManagedContainer
{
    public string ContainerId { get; init; } = string.Empty;
    public Guid? LabId { get; init; }
}