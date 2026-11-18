using Microsoft.EntityFrameworkCore;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly PwnlabsDbContext _db;

    public RoomRepository(PwnlabsDbContext db)
    {
        _db = db;
    }

    public async Task<List<Room>> GetPublishedAsync()
    {
        var rooms = await _db.Rooms
            .Where(r => r.IsPublished)
            .ToListAsync();

        return rooms.OrderBy(r => r.Category).ThenBy(r => r.Difficulty).ToList();
    }

    public async Task<Room?> GetPublishedBySlugAsync(string slug)
    {
        return await _db.Rooms
            .FirstOrDefaultAsync(r => r.Slug == slug && r.IsPublished);
    }
}

