using Pwnlabs.Domain.Enums;

namespace Pwnlabs.Domain.Entities;

public class LabInstance
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required Guid UserId { get; set; } 
    public User? User { get; set; }
    public required string ContainerId { get; set; }
    public required string Image { get; set; }
    public int HostPort { get; set; }
    public LabStatus Status { get; set; } = LabStatus.Running;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime ExpiresAt { get; set; }
    public DateTime? StoppedAt { get; set; }

    // Room relation
    public Guid RoomId { get; set; }
    public Room? Room { get; set; }
}