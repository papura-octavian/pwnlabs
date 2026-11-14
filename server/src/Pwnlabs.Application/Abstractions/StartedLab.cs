namespace Pwnlabs.Application.Abstractions;

public record StartedLab
{
    public string ContainerId { get; init; } = string.Empty;
    public int HostPort  { get; init; }
}