using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace Client.Middleware;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthMessageHandler> _logger;

    public AuthMessageHandler(ILocalStorageService localStorage, ILogger<AuthMessageHandler> logger)
    {
        _localStorage = localStorage;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _logger.LogTrace("Added Authorization header to outgoing request.");
        }
        else
        {
            _logger.LogTrace("No token found in local storage; Authorization header not added.");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}