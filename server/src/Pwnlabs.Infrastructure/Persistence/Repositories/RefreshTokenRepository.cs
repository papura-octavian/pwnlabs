using Microsoft.EntityFrameworkCore;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Persistence.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PwnlabsDbContext _db;

    public RefreshTokenRepository(PwnlabsDbContext db)
    {
        _db = db;
    }

    public void Add(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        return await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    }

    public async Task RevokeAllForUserAsync(Guid userId)
    {
        await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
    }
}