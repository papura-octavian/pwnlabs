namespace Pwnlabs.Application.Labs.Dtos;

public record LabResponse
{
    public Guid Id { get; init; }
    public string Image  { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
}