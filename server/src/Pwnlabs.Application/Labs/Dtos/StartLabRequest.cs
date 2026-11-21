using System.ComponentModel.DataAnnotations;

namespace Pwnlabs.Application.Labs.Dtos;

public record StartLabRequest
{
    [Required]
    public string RoomSlug { get; init; } = string.Empty;
}