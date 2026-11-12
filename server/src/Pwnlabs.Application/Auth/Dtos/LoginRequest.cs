using System.ComponentModel.DataAnnotations;

namespace Pwnlabs.Application.Auth.Dtos;

public record LoginRequest
{
    [Required]
    [StringLength(254)]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string Password { get; init; } = string.Empty;
}