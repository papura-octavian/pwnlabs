using Pwnlabs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pwnlabs.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Username settings
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(16);

        builder.HasIndex(u => u.Username)
            .IsUnique();
        
        // Email settings
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // PasswordHash settings
        builder.Property(u => u.PasswordHash)
            .IsRequired();

        // enum settings
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(16);
    }
}