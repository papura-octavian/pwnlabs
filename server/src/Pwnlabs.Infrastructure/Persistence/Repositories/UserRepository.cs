using Microsoft.EntityFrameworkCore;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PwnlabsDbContext _db;

    public UserRepository(PwnlabsDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string normalizedUsername)
    {
        return await _db.Users.AnyAsync(u => u.NormalizedUsername == normalizedUsername);
    }

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

}