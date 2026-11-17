using Microsoft.EntityFrameworkCore;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;
using Pwnlabs.Domain.Enums;

namespace Pwnlabs.Infrastructure.Persistence.Repositories;

public class LabRepository : ILabRepository
{
    private readonly PwnlabsDbContext _db;

    public LabRepository(PwnlabsDbContext db)
    {
        _db = db;
    }

    public void Add(LabInstance lab)
    {
        _db.LabInstances.Add(lab);
    }

    public async Task<LabInstance?> GetByIdAsync(Guid id)
    {
        return await _db.LabInstances.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<LabInstance>> GetActiveByUserAsync(Guid userId)
    {
        return await _db.LabInstances
            .Where(l => l.UserId == userId && l.Status == LabStatus.Running)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> CountActiveByUserAsync(Guid userId)
    {
        return await _db.LabInstances.CountAsync(l => l.UserId == userId && l.Status == LabStatus.Running);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<LabInstance>> GetExpiredRunningAsync(DateTime now)
    {
        return await _db.LabInstances
            .Where(l => l.Status == LabStatus.Running && l.ExpiresAt <= now)
            .ToListAsync();
    }
}