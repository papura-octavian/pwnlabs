namespace Pwnlabs.Application.Auth.Dtos;
using System.ComponentModel.DataAnnotations;

public record RegisterRequest
{
    [Required]
    [StringLength(16, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z0-9_-]+$")]
    public string Username { get; init; }  = string.Empty;

    [Required]
    [StringLength(256)]
    [EmailAddress]
    public string Email { get; init; }  = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; init; }  = string.Empty;
}