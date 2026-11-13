using Pwnlabs.Application.Auth.Dtos;

namespace Pwnlabs.Application.Auth;

public class AuthResult
{
    public AuthResponse Response { get; init; } = new();
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; init; }
}