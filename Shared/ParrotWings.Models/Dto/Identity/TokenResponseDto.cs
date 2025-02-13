using System.Diagnostics.CodeAnalysis;

namespace ParrotWings.Models.Dto.Identity;

/// <summary>
/// Model for user login.
/// </summary>
public class TokenResponseDto
{
    public string JwtToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}