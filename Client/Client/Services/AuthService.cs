using System.Net;
using System.Net.Http.Json;
using Client.Helper;
using Client.Identity;
using Client.Identity.Models;
using Client.Services.Interfaces;
using ParrotWings.Models.Dto.Identity;

namespace Client.Services;

public class AuthService : IAuthService
{
    private readonly PwAuthenticationStateProvider _authenticationStateProvider;
    private readonly ILogger<AuthService> _logger;
    private readonly HttpClient _httpClient;

    public AuthService(
        IHttpClientFactory clientFactory,
        PwAuthenticationStateProvider authenticationStateProvider,
        ILogger<AuthService> logger)
    {
        var clientFactory1 = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = clientFactory1.CreateClient("API");
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<RequestResult> RegisterAsync(string name, string email, string password)
    {
        try
        {
            _logger.LogInformation("Attempting registration for email {Email}", email);

            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new RegisterRequestDto
            {
                Email = email,
                FullName = name,
                Password = password
            }, RegisterRequestDtoJsonContext.Default.RegisterRequestDto);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>(
                    TokenResponseDtoJsonContext.Default.TokenResponseDto);

                if (tokenResponse == null)
                {
                    _logger.LogWarning("Registration failed for {Email}: No token returned", email);
                    return new RequestResult { Succeeded = false, ErrorList = ["An error occurred during registration."] };
                }

                await _authenticationStateProvider.MarkUserAsAuthenticatedAsync(tokenResponse.JwtToken, tokenResponse.RefreshToken);
                _logger.LogInformation("Registration successful for {Email}", email);
                return new RequestResult { Succeeded = true };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Registration failed for {Email}: {StatusCode} - {Error}", email,
                    response.StatusCode, error);
                return new RequestResult { Succeeded = false, ErrorList = [error] };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration for {Email}", email);
            return new RequestResult { Succeeded = false, ErrorList = ["An error occurred during registration."] };
        }
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    public async Task<RequestResult> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("Attempting login for {Email}", email);

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Email = email,
                Password = password
            },
            LoginRequestDtoJsonContext.Default.LoginRequestDto);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseDto>(TokenResponseDtoJsonContext.Default.TokenResponseDto);
                if (tokenResponse == null)
                {
                    _logger.LogWarning("Login failed for {Email}: No token returned", email);
                    return new RequestResult { Succeeded = false, ErrorList = ["An error occurred during login."] };
                }

                await _authenticationStateProvider.MarkUserAsAuthenticatedAsync(tokenResponse.JwtToken, tokenResponse.RefreshToken);
                _logger.LogInformation("Login successful for {Email}", email);
                return new RequestResult { Succeeded = true };
            }

            var error = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(error))
            {
                error = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Invalid username or password.",
                    _ => "An error occurred during login."
                };
            }

            _logger.LogWarning("Login failed for {Email}: {StatusCode} - {Error}", email, response.StatusCode,
                error);
            return new RequestResult { Succeeded = false, ErrorList = [error] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for {Email}", email);
            return new RequestResult { Succeeded = false, ErrorList = ["An error occurred during login."] };
        }
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    public async Task<RequestResult> LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Attempting logout.");
            // В данном примере предполагается, что для logout не требуется тело запроса.
            var response = await _httpClient.PostAsync("/api/auth/logout", new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
            {
                await _authenticationStateProvider.MarkUserAsLoggedOutAsync();
                _logger.LogInformation("Logout successful.");
                return new RequestResult { Succeeded = true };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Logout failed: {StatusCode} - {Error}", response.StatusCode, error);
            return new RequestResult { Succeeded = false, ErrorList = [error] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during logout.");
            return new RequestResult { Succeeded = false, ErrorList = ["An error occurred during logout."] };
        }
    }
}