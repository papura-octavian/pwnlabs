using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Auth.Dtos;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();
        var normalizedUsername = username.ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new ConflictException("Email is already registered.");
        }

        if (await _userRepository.ExistsByUsernameAsync(normalizedUsername))
        {
            throw new ConflictException("Username is already taken.");
        }

        var user = new User
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _userRepository.AddAsync(user);

        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var hash = _tokenService.HashToken(refreshToken);
        var stored = await _refreshTokenRepository.GetByHashAsync(hash);

        if (stored is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (stored.RevokedAt is not null)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(stored.UserId);
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (stored.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        stored.RevokedAt = DateTime.UtcNow;
        return await IssueTokensAsync(stored.User!);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var hash = _tokenService.HashToken(refreshToken);
        var stored = await _refreshTokenRepository.GetByHashAsync(hash);

        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.SaveChangesAsync();
        }
    }

    private async Task<AuthResult> IssueTokensAsync(User user)
    {
        var access  = _tokenService.CreateAccessToken(user);
        var refresh = _tokenService.GenerateRefreshToken();

        _refreshTokenRepository.Add(new RefreshToken
        {
            UserId    = user.Id,
            TokenHash = refresh.TokenHash,
            ExpiresAt = refresh.ExpiresAt
        });
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResult
        {
            Response = access,
            RefreshToken = refresh.Token,
            RefreshTokenExpiresAt = refresh.ExpiresAt
        };
    }
}