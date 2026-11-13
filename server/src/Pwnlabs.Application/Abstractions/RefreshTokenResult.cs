namespace Pwnlabs.Application.Abstractions;

public record RefreshTokenResult
{
    public string Token { get; init; } = string.Empty;
    public string TokenHash { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}