using Pwnlabs.Application.Auth.Dtos;

namespace Pwnlabs.Application.Auth;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request);
}