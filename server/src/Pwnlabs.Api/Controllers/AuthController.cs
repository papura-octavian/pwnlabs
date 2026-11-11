using Microsoft.AspNetCore.Mvc;
using Pwnlabs.Application.Auth;
using Pwnlabs.Application.Auth.Dtos;

namespace Pwnlabs.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest registerRequest)
    {
        var response = await _authService.RegisterAsync(registerRequest);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}