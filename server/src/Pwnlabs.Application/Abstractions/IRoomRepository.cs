using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Abstractions;

public interface IRoomRepository
{
    Task<List<Room>> GetPublishedAsync();
    Task<Room?> GetPublishedBySlugAsync(string slug);
}