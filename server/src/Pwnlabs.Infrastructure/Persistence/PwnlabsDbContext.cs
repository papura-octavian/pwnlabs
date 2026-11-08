using Microsoft.EntityFrameworkCore;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Persistence;

public class PwnlabsDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public PwnlabsDbContext(DbContextOptions<PwnlabsDbContext> options) : base(options) {}
}