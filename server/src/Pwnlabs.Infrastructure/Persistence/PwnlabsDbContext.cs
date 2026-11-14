using Microsoft.EntityFrameworkCore;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Persistence;

public class PwnlabsDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LabInstance> LabInstances => Set<LabInstance>();
    public PwnlabsDbContext(DbContextOptions<PwnlabsDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PwnlabsDbContext).Assembly);
    }
}