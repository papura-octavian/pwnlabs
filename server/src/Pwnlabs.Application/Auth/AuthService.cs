using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Auth.Dtos;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new ConflictException("Email is already registered.");
        }

        if (await _userRepository.ExistsByUsernameAsync(username))
        {
            throw new ConflictException("Username is already taken");
        }

        var user = new User
        {
            Username = username,
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

}