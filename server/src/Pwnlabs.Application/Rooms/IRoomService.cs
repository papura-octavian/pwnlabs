using Pwnlabs.Application.Rooms.Dtos;

namespace Pwnlabs.Application.Rooms;

public interface IRoomService
{
    Task<List<RoomResponse>> GetAllAsync();
    Task<RoomResponse> GetBySlugAsync(string slug);
}