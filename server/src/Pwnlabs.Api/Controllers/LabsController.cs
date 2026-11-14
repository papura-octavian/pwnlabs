using Microsoft.AspNetCore.Mvc;
using Pwnlabs.Application.Labs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Pwnlabs.Application.Labs.Dtos;

namespace Pwnlabs.Api.Controllers;

[ApiController]
[Route("api/v1/labs")]
[Authorize]
public class LabsController : ControllerBase
{
    private readonly ILabService _labService;
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue("sub")!);

    public LabsController(ILabService labService)
    {
        _labService = labService;
    }

    [HttpPost]
    public async Task<ActionResult<LabResponse>> Start()
    {
        var response = await _labService.StartAsync(CurrentUserId);
        return StatusCode(
            StatusCodes.Status201Created,
            response
        );
    }

    [HttpGet]
    public async Task<ActionResult<List<LabResponse>>> GetMine()
    {
        var response = await _labService.GetMyLabsAsync(CurrentUserId);
        return Ok(response);
    }

    [HttpDelete("{labId:guid}")]
    public async Task<IActionResult> Stop(Guid labId)
    {
        await _labService.StopAsync(CurrentUserId, labId);

        return NoContent();
    }
}