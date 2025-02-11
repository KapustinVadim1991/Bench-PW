using System.Diagnostics.CodeAnalysis;

namespace ParrotWings.Models.Dto.Identity;

/// <summary>
/// Model for user login.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class LoginRequestDto
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>
    /// If cookie is used
    /// </summary>
    public bool UseCookie { get; set; }

    /// <summary>
    /// If cookie keeps stored in browser after closing
    /// </summary>
    public bool IsPresistCookie { get; set; }
}