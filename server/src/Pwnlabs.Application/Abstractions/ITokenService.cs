using Pwnlabs.Application.Auth.Dtos;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Abstractions;

public interface ITokenService
{
    AuthResponse CreateAccessToken(User user);
    RefreshTokenResult GenerateRefreshToken();
    string HashToken(string token);
}