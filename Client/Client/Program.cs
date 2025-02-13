using Blazored.LocalStorage;
using Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client.Identity;
using Client.Middleware;
using Client.Services;
using Client.Services.Interfaces;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

// set up authorization
builder.Services.AddAuthorizationCore();

// register the custom state provider
builder.Services.AddScoped<PwAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<PwAuthenticationStateProvider>());

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthMessageHandler>();

// set base address for default host
builder.Services.AddScoped(_ =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "http://localhost:5002") });

// configure client for auth interactions
builder.Services.AddHttpClient(
    "API",
    opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "http://localhost:5001"))
    .AddHttpMessageHandler<AuthMessageHandler>();

await builder.Build().RunAsync();