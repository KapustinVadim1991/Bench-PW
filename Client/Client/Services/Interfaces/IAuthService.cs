using Client.Identity.Models;

namespace Client.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="name">The user's full name.</param>
    /// <param name="email">The user's email.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>A FormResult indicating success or failure.</returns>
    Task<FormResult> RegisterAsync(string name, string email, string password);

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <returns>A FormResult indicating success or failure.</returns>
    Task<FormResult> LoginAsync(string email, string password);

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <returns>A FormResult indicating success or failure.</returns>
    Task<FormResult> LogoutAsync();

    /// <summary>
    /// Retrieves current user info.
    /// </summary>
    /// <returns>
    /// Returns user info is successfully retrieved
    /// </returns>
    Task<FormResult<UserInfo>> GetUserInfoAsync();
}