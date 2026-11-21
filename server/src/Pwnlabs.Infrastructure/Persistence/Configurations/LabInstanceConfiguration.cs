using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Persistence.Configurations;

public class LabInstanceConfiguration : IEntityTypeConfiguration<LabInstance>
{
    public void Configure(EntityTypeBuilder<LabInstance> builder)
    {
        // Primary key
        builder.HasKey(l => l.Id);

        // ContainerId
        builder.Property(l => l.ContainerId)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(l => l.ContainerId)
            .IsUnique();    

        // Image
        builder.Property(l => l.Image)
            .IsRequired()
            .HasMaxLength(256);

        // Status
        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(16);

        // User relation
        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Room relation
        builder.HasOne(l => l.Room)
            .WithMany()
            .HasForeignKey(l => l.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}