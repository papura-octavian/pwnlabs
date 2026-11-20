using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pwnlabs.Application.Rooms;
using Pwnlabs.Domain.Entities;
using Pwnlabs.Application.Rooms.Dtos;

namespace Pwnlabs.Api.Controllers;

[ApiController]
[Route("api/v1/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomResponse>>> GetAll()
    {
        var response = await _roomService.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<RoomResponse>> GetBySlug(string slug)
    {
        var response = await _roomService.GetBySlugAsync(slug);

        return Ok(response);
    }
}