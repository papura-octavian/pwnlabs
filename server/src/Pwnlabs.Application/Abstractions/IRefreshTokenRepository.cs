using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Abstractions;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);
    Task<RefreshToken?> GetByHashAsync(string tokenHash);
    Task RevokeAllForUserAsync(Guid userId);
    Task SaveChangesAsync();
}