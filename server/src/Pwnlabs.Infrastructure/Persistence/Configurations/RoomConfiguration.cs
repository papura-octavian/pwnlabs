using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pwnlabs.Domain.Entities;
using Pwnlabs.Domain.Enums;

namespace Pwnlabs.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        // Primary Key
        builder.HasKey(r => r.Id);

        // Slug Configuration
        builder.Property(r => r.Slug)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(r => r.Slug)
            .IsUnique();

        // Title Configuration
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(128);

        // Description Configuration
        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(4000);

        // Image Configuration
        builder.Property(r => r.Image)
            .IsRequired()
            .HasMaxLength(256);

        // Category Configuration
        builder.Property(r => r.Category)
            .HasConversion<string>()
            .HasMaxLength(16);
        
        // Difficulty Configuration
        builder.Property(r => r.Difficulty)
            .HasConversion<string>()
            .HasMaxLength(16);

        // ==== Temporary Testing ====
        builder.HasData(new Room
        {
            Id = Guid.Parse("01a1f000-0000-7000-8000-000000000001"),
            Slug = "nginx-welcome",
            Title = "Welcome to nginx",
            Description = "A test room that runs a plain nginx server.",
            Category = RoomCategory.Web,
            Difficulty = RoomDifficulty.Easy,
            Points = 10,
            Image = "nginx:alpine",
            ContainerPort = 80,
            IsPublished = true,
            CreatedAt = new DateTime(2026, 11, 18, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}