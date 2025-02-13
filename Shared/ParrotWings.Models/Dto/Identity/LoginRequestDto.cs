using System.Diagnostics.CodeAnalysis;

namespace ParrotWings.Models.Dto.Identity;

/// <summary>
/// Model for user login.
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}