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

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
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

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        return _tokenService.CreateAccessToken(user);
    }
}