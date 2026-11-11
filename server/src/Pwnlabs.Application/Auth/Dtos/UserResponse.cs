namespace Pwnlabs.Application.Auth.Dtos;

public record UserResponse
{
    public Guid Id { get; init; } 
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; }  = string.Empty;
    public string Role { get; init; }  = string.Empty;
    public DateTime CreatedAt { get; init; } 
}