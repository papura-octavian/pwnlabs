namespace Pwnlabs.Application.Auth.Dtos;

public record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}