namespace Pwnlabs.Application.Rooms.Dtos;

public record RoomResponse
{
    public string Slug { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Difficulty { get; init; } = string.Empty;
    public int Points { get; init; }
}