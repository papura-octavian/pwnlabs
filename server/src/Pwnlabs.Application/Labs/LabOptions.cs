namespace Pwnlabs.Application.Labs;

public class LabOptions
{
    public string DefaultImage { get; set; } = string.Empty;
    public int ContainerPort  { get; set; } 
    public int MemoryLimitMb  { get; set; }
    public int LifetimeMinutes  { get; set; }
    public int MaxActiveLabsPerUser  { get; set; }
}