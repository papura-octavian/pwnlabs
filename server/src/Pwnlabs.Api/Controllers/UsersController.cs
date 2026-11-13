using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pwnlabs.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]

public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        return Ok(new
        {
            Id = User.FindFirstValue("sub"),
            Username = User.FindFirstValue("username"),
            Email = User.FindFirstValue("email"),
            Role = User.FindFirstValue("role")
        });
    }
}