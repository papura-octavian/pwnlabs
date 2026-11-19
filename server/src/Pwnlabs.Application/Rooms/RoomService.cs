using Pwnlabs.Application.Rooms.Dtos;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Common.Exceptions;

namespace Pwnlabs.Application.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task<List<RoomResponse>> GetAllAsync()
    {
        throw new NotImplementedException("Function is not implemented!");
    }
    public async Task<RoomResponse> GetBySlugAsync(string slug)
    {
        var room = await _roomRepository.GetPublishedBySlugAsync(slug);
        
        if (room is null)
        {
            throw new NotFoundException("Room not found.");
        }

        return new RoomResponse
        {
            Slug = room.Slug,
            Title = room.Title,
            Description = room.Description,
            Category = room.Category.ToString(),
            Difficulty = room.Difficulty.ToString(),
            Points = room.Points
        };
    }
}