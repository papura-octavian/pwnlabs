using NSubstitute;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Auth;
using Pwnlabs.Application.Auth.Dtos;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Tests.Auth;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository, _passwordHasher, _tokenService, _refreshTokenRepository);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ThrowsConflictException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "CorrectHorse123"
        };
        _userRepository.ExistsByEmailAsync("alice@example.com").Returns(true);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() => _sut.RegisterAsync(request));

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }
}