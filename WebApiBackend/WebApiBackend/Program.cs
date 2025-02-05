using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Database;
using WebApiBackend.Identity;
using WebApiBackend.Identity.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PwDbContext>(
    options => options.UseInMemoryDatabase("PwDb"));
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<PwDbContext>();

builder.Services.AddAuthorizationBuilder();
// add a CORS policy for the client
builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins([
                builder.Configuration["BackendUrl"] ?? "http://localhost:5001",
                builder.Configuration["FrontendUrl"] ?? "http://localhost:5002"
            ])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
    app.UseDeveloperExceptionPage();
}

app.UseCors("wasm");

// Enable authentication and authorization after CORS Middleware
// processing (UseCors) in case the Authorization Middleware tries
// to initiate a challenge before the CORS Middleware has a chance
// to set the appropriate headers.
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<AppUser>();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", (HttpContext httpContext) =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = 0
                })
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi()
    .RequireAuthorization();

app.MapPost("/registration", async Task<Results<Ok, ValidationProblem>>
(
    [FromBody] RegisterRequestExt registration,
    HttpContext context,
    [FromServices] IServiceProvider sp) =>
{
    var userManager = sp.GetRequiredService<UserManager<AppUser>>();

    if (!userManager.SupportsUserEmail)
    {
        throw new NotSupportedException("Requires a user store with email support.");
    }

    var userStore = sp.GetRequiredService<IUserStore<AppUser>>();
    var emailStore = (IUserEmailStore<AppUser>)userStore;
    var email = registration.Email;

    var emailAddressAttribute = new EmailAddressAttribute();
    if (string.IsNullOrEmpty(email) || !emailAddressAttribute.IsValid(email))
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { "Invalid email", ["Email address is not valid."] }
        });
    }

    var user = new AppUser();
    await userStore.SetUserNameAsync(user, registration.Name, CancellationToken.None);
    await emailStore.SetEmailAsync(user, email, CancellationToken.None);
    var result = await userManager.CreateAsync(user, registration.Password);

    if (!result.Succeeded)
    {
        return result.CreateValidationProblem();
    }

    return TypedResults.Ok();
});

app.MapPost("/logout", async (SignInManager<AppUser> signInManager, [FromBody] object? empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }

    return Results.Unauthorized();
}).RequireAuthorization();


app.MapGet("/test", (HttpContext httpContext) => "test response for test request");

app.Run();