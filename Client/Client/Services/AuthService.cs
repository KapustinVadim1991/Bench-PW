using System.Net.Http.Json;
using Client.Helper;
using Client.Identity.Models;
using Client.Services.Interfaces;
using ParrotWings.Models.Dto.Identity;

namespace Client.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly HttpClient _httpClient;

    public AuthService(
        IHttpClientFactory clientFactory,
        ILogger<AuthService> logger)
    {
        var clientFactory1 = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = clientFactory1.CreateClient("Auth");
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public async Task<FormResult> RegisterAsync(string name, string email, string password)
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
                _logger.LogInformation("Registration successful for {Email}", email);
                return new FormResult { Succeeded = true };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Registration failed for {Email}: {StatusCode} - {Error}", email,
                    response.StatusCode, error);
                return new FormResult { Succeeded = false, ErrorList = [error] };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during registration for {Email}", email);
            return new FormResult { Succeeded = false, ErrorList = ["An error occurred during registration."] };
        }
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    public async Task<FormResult> LoginAsync(string email, string password)
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
                _logger.LogInformation("Login successful for {Email}", email);
                return new FormResult { Succeeded = true };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Login failed for {Email}: {StatusCode} - {Error}", email, response.StatusCode,
                error);
            return new FormResult { Succeeded = false, ErrorList = [error] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for {Email}", email);
            return new FormResult { Succeeded = false, ErrorList = ["An error occurred during login."] };
        }
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    public async Task<FormResult> LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Attempting logout.");
            // В данном примере предполагается, что для logout не требуется тело запроса.
            var response = await _httpClient.PostAsync("/api/auth/logout", new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Logout successful.");
                return new FormResult { Succeeded = true };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Logout failed: {StatusCode} - {Error}", response.StatusCode, error);
            return new FormResult { Succeeded = false, ErrorList = [error] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during logout.");
            return new FormResult { Succeeded = false, ErrorList = ["An error occurred during logout."] };
        }
    }

    /// <summary>
    /// Retrieves current user info.
    /// </summary>
    public async Task<FormResult<UserInfo>> GetUserInfoAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving user info.");
            var response = await _httpClient.GetAsync("/api/auth/info");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User exists.");
                return new FormResult<UserInfo>
                {
                    Succeeded = true,
                    Data = await response.Content.ReadFromJsonAsync<UserInfo>(JsonContext.Default.UserInfo)
                };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to retrieve user info: {StatusCode} - {Error}", response.StatusCode, error);
            return new FormResult<UserInfo> { Succeeded = false, ErrorList = [error] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user info.");
            return new FormResult<UserInfo> { Succeeded = false, ErrorList = [ex.Message] };
        }
    }
}