using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Client.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ParrotWings.Models.Dto.Identity;

namespace Client.Identity;

public class PwAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<PwAuthenticationStateProvider> _logger;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;

    public PwAuthenticationStateProvider(
        ILocalStorageService localStorage,
        ILogger<PwAuthenticationStateProvider> logger,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigationManager)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient("API");
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// Returns the current authentication state. If the token is missing or expired and the refresh fails, the user is considered unauthenticated.
    /// If the token is expired, it attempts to refresh it.
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Retrieve the saved JWT token from localStorage
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            // No token - user is unauthenticated
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Parse the token
        var jwtHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = jwtHandler.ReadJwtToken(token);
        }
        catch
        {
            // If the token is invalid - clear the authentication state
            await MarkUserAsLoggedOutAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Check the token's expiration (the "exp" claim contains Unix time)
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
        if (expClaim != null && long.TryParse(expClaim, out var expUnix))
        {
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            if (expirationTime < DateTime.UtcNow)
            {
                // Token has expired - attempt to refresh
                var newToken = await TryRefreshTokenAsync();
                if (string.IsNullOrWhiteSpace(newToken))
                {
                    // Refresh failed - log out the user
                    await MarkUserAsLoggedOutAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                else
                {
                    token = newToken;
                }
            }
        }

        // Create a ClaimsIdentity from the valid token
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    /// <summary>
    /// Called upon successful login. Saves the JWT and refresh token, sets the HttpClient authorization header, and notifies the system of the authentication state change.
    /// </summary>
    public async Task MarkUserAsAuthenticatedAsync(string token, string refreshToken)
    {
        await _localStorage.SetItemAsync("authToken", token);
        await _localStorage.SetItemAsync("refreshToken", refreshToken);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    /// <summary>
    /// Clears the stored tokens, resets the HttpClient authorization header, and notifies the system that the user has been logged out.
    /// </summary>
    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");

        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));

        _navigationManager.NavigateTo("login");
    }

    /// <summary>
    /// Attempts to refresh the JWT by sending the refresh token to the API.
    /// If successful, it updates the stored tokens and returns the new JWT.
    /// </summary>
    private async Task<string?> TryRefreshTokenAsync()
    {
        var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        // Build the request for token refresh
        var requestData = new TokenRefreshRequestDto
        {
            RefreshToken = refreshToken
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/auth/refresh",
                requestData,
                TokenRefreshRequestDtoJsonContext.Default.TokenRefreshRequestDto
            );

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>(TokenResponseDtoJsonContext.Default.TokenResponseDto);
                if (tokenResponse != null)
                {
                    // Update the saved tokens and HttpClient authorization header
                    await _localStorage.SetItemAsync("authToken", tokenResponse.JwtToken);
                    await _localStorage.SetItemAsync("refreshToken", tokenResponse.RefreshToken);
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", tokenResponse.JwtToken);
                    return tokenResponse.JwtToken;
                }
            }
        }
        catch
        {
            // Optionally log errors here
            _logger.LogError("Failed to refresh token.");
            throw;
        }

        return null;
    }

    /// <summary>
    /// Parses the JWT and returns a collection of claims.
    /// </summary>
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}