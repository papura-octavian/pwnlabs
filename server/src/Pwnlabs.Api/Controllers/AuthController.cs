using Microsoft.AspNetCore.Mvc;
using Pwnlabs.Application.Auth;
using Pwnlabs.Application.Auth.Dtos;

namespace Pwnlabs.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private const string RefreshCookieName = "refreshToken";

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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);
        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
        return StatusCode(StatusCodes.Status200OK, result.Response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        var token = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(token);
        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAt);
        return Ok(result.Response);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var token = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(token))
        {
            await _authService.LogoutAsync(token);
        }

        Response.Cookies.Delete(RefreshCookieName, new CookieOptions 
        { 
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
        });
        return NoContent();
    }

    private void SetRefreshCookie(string token, DateTime expiresAt)
    {
        Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
            Expires = expiresAt
        });
    }
}