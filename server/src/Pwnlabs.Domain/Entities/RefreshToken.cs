namespace Pwnlabs.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required Guid UserId { get; set; } 
    public User? User { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public DateTime? RevokedAt { get; set; }
}