using Pwnlabs.Domain.Enums;

namespace Pwnlabs.Domain.Entities;

public class Room
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public RoomCategory Category { get; set; }
    public RoomDifficulty Difficulty { get; set; }
    public int Points { get; set; }
    public required string Image { get; set; }
    public int ContainerPort { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}