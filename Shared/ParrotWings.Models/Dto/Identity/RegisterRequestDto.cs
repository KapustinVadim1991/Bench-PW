namespace ParrotWings.Models.Dto.Identity;

/// <summary>
/// Model for registering a new user.
/// </summary>
public class RegisterRequestDto
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}