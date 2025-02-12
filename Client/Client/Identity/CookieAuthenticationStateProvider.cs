using System.Security.Claims;
using System.Text.Json;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Identity;

/// <summary>
/// Handles state for cookie-based auth.
/// </summary>
/// <remarks>
/// Create a new instance of the auth provider.
/// </remarks>
/// <param name="httpClientFactory">Factory to retrieve auth client.</param>
public class CookieAuthenticationStateProvider(
    IHttpClientFactory httpClientFactory,
    ILogger<CookieAuthenticationStateProvider> logger,
    IAuthService authService
    )
    : AuthenticationStateProvider
{
    /// <summary>
    /// Map the JavaScript-formatted properties to C#-formatted classes.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    /// <summary>
    /// Special auth client.
    /// </summary>
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");

    /// <summary>
    /// Authentication state.
    /// </summary>
    private bool _authenticated;

    /// <summary>
    /// Default principal for anonymous (not authenticated) users.
    /// </summary>
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    /// <summary>
    /// Get authentication state.
    /// </summary>
    /// <remarks>
    /// Called by Blazor anytime and authentication-based decision needs to be made, then cached
    /// until the changed state notification is raised.
    /// </remarks>
    /// <returns>The authentication state asynchronous request.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        // default to not authenticated
        var user = _unauthenticated;

        try
        {
            // the user info endpoint is secured, so if the user isn't logged in this will fail
            var userResponse = await authService.GetUserInfoAsync();

            if (!userResponse.Succeeded)
            {
                logger.LogWarning("User is not authenticated");
                return new AuthenticationState(user);
            }

            // user is authenticated,so let's build their authenticated identity
            if (userResponse.Data != null)
            {
                var userInfo = userResponse.Data;
                // in this example app, name and email are the same
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userInfo.Name),
                    new(ClaimTypes.Email, userInfo.Email),
                };

                // add any additional claims
                claims.AddRange(
                    userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                        .Select(c => new Claim(c.Key, c.Value)));

                // set the principal
                var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                user = new ClaimsPrincipal(id);
                _authenticated = true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "App error");
        }

        // return the state
        return new AuthenticationState(user);
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _authenticated;
    }
}