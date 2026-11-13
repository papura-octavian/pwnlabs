using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Auth.Dtos;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthResponse CreateAccessToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username", user.Username),
            new("role", user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            SigningCredentials = credentials
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AuthResponse
        {
            AccessToken = token,
            ExpiresAt = expiresAt
        };
    }

    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public RefreshTokenResult GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToHexString(bytes);
        return new RefreshTokenResult
        {
            Token = token,
            TokenHash = HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays)  
        };
    }
}