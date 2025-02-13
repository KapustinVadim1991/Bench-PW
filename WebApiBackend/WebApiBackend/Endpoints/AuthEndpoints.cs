using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParrotWings.Models.Dto.Identity;
using ParrotWings.Models.Dto.Users;
using WebApiBackend.Database;
using WebApiBackend.Database.Domain;
using WebApiBackend.Extensions;
using WebApiBackend.Service.Token;

namespace WebApiBackend.Endpoints
{
    /// <summary>
    /// Contains authentication endpoints: register, login, and logout.
    /// </summary>
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
        {
            routes.MapPost(
                "/api/auth/register",
                async Task<Results<Ok<TokenResponseDto>, BadRequest<string>, BadRequest<ValidationProblem>>>(
                    [FromBody] RegisterRequestDto model,
                    [FromServices] ITokenService tokenService,
                    HttpContext httpContext,
                    UserManager<AppUser> userManager,
                    PwDbContext context,
                    ILogger<Endpoints> logger) =>
            {
                logger.LogInformation("Registration attempt for email {Email}", model.Email);

                // Check if a user with the provided email already exists.
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    logger.LogWarning("Registration failed: a user with email {Email} already exists.", model.Email);
                    return TypedResults.BadRequest("A user with this email already exists.");
                }

                // Create a new user with the provided details and a starting balance of 500.
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Balance = 500
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogWarning("Registration failed for {Email}: {Errors}", model.Email, errors);
                    return TypedResults.BadRequest(result.CreateValidationProblem());
                }

                var token = tokenService.GenerateJwtToken(user);
                var refreshToken = tokenService.GenerateRefreshToken(httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
                refreshToken.UserId = user.Id;

                // Сохраняем refresh-token в БД
                context.RefreshTokens.Add(refreshToken);
                await context.SaveChangesAsync();

                logger.LogInformation("User registered successfully: {Email}", model.Email);

                return TypedResults.Ok(new TokenResponseDto
                {
                    JwtToken = token,
                    RefreshToken = refreshToken.Token
                });
            });

            // Login endpoint: POST /api/auth/login
            routes.MapPost("/api/auth/login", async Task<Results<Ok<TokenResponseDto>, UnauthorizedHttpResult>>(
                [FromBody] LoginRequestDto model,
                [FromServices] IConfiguration configuration,
                [FromServices] ITokenService tokenService,
                HttpContext httpContext,
                UserManager<AppUser> userManager,
                PwDbContext context,
                ILogger<Endpoints> logger) =>
            {
                logger.LogInformation("Login attempt for {Email}", model.Email);

                var user = await userManager.FindByNameAsync(model.Email);
                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                {
                    return TypedResults.Unauthorized();
                }

                var token = tokenService.GenerateJwtToken(user);
                var refreshToken = tokenService.GenerateRefreshToken(httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
                refreshToken.UserId = user.Id;

                context.RefreshTokens.Add(refreshToken);
                await context.SaveChangesAsync();

                logger.LogInformation("User logged in successfully: {Email}", model.Email);

                return TypedResults.Ok(new TokenResponseDto
                {
                    JwtToken = token,
                    RefreshToken = refreshToken.Token
                });
            }).AllowAnonymous();

            // Logout endpoint: POST /api/auth/logout
            routes.MapPost("/api/auth/logout", async Task<Results<Ok<string>, BadRequest<string>>> (
                [FromQuery] string email,
                UserManager<AppUser> userManager,
                HttpContext httpContext,
                PwDbContext context,
                ILogger<Endpoints> logger) =>
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return TypedResults.BadRequest("User not found.");
                }

                var existingTokens = await context.RefreshTokens
                    .Where(rt => rt.UserId == user.Id && rt.IsActive)
                    .ToListAsync();

                if (!existingTokens.Any())
                {
                    return TypedResults.Ok("No refresh tokens found.");
                }

                foreach (var token in existingTokens)
                {
                    token.Revoked = DateTime.UtcNow;
                    token.RevokedByIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                }

                await context.SaveChangesAsync();
                logger.LogInformation("User logged out successfully.");
                return TypedResults.Ok("Logout successful.");
            });

            // Info endpoint: GET /api/auth/info
            routes.MapGet("/api/auth/info", async Task<Results<Ok<UserDto>, ValidationProblem, NotFound>>(
                ClaimsPrincipal claimsPrincipal,
                [FromServices] IServiceProvider sp,
                ILogger<Endpoints> logger) =>
            {
                logger.LogInformation("Retrieving user info for {UserName}", claimsPrincipal.Identity?.Name);

                var userManager = sp.GetRequiredService<UserManager<AppUser>>();
                var user = await userManager.GetUserAsync(claimsPrincipal);
                if (user is null)
                {
                    logger.LogWarning("User info retrieval failed: user not found.");
                    return TypedResults.NotFound();
                }

                logger.LogInformation("User info retrieved successfully for {Email}", user.Email);
                return TypedResults.Ok(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                });
            });

            routes.MapPost("/api/auth/refresh", async Task<Results<Ok<TokenResponseDto>, BadRequest<string>>> (
                [FromBody] TokenRefreshRequestDto tokenRefreshRequest,
                [FromServices] ITokenService tokenService,
                HttpContext httpContext,
                PwDbContext context) =>
            {
                // Находим refresh-token в базе (включая связанного пользователя)
                var existingToken = await context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == tokenRefreshRequest.RefreshToken);

                if (existingToken == null || !existingToken.IsActive)
                {
                    return TypedResults.BadRequest("Invalid refresh token.");
                }

                // Отмечаем старый refresh-token как отозванный
                existingToken.Revoked = DateTime.UtcNow;
                existingToken.RevokedByIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                await context.SaveChangesAsync();

                var newJwtToken = tokenService.GenerateJwtToken(existingToken.User);
                var newRefreshToken = tokenService.GenerateRefreshToken(httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
                newRefreshToken.UserId = existingToken.UserId;

                context.RefreshTokens.Add(newRefreshToken);
                await context.SaveChangesAsync();

                return TypedResults.Ok(new TokenResponseDto
                {
                    JwtToken = newJwtToken,
                    RefreshToken = newRefreshToken.Token
                });
            }).AllowAnonymous();

            return routes;
        }
    }
}