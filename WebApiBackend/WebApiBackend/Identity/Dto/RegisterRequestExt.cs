namespace WebApiBackend.Identity.Dto;

public sealed class RegisterRequestExt
{
    /// <summary>
    /// The user's name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
}