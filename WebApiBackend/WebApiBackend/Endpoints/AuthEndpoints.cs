using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParrotWings.Models.Dto.Identity;
using ParrotWings.Models.Dto.Users;
using WebApiBackend.Database.Domain;
using WebApiBackend.Extensions;

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
                async Task<Results<Ok<string>, BadRequest<string>, BadRequest<ValidationProblem>>>(
                    [FromBody] RegisterRequestDto model,
                    UserManager<AppUser> userManager,
                    SignInManager<AppUser> signInManager,
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

                // Sign in the user (set authentication cookies)
                await signInManager.SignInAsync(user, isPersistent: false);
                logger.LogInformation("User registered and signed in successfully: {Email}", model.Email);
                return TypedResults.Ok("Registration successful.");
            });

            // Login endpoint: POST /api/auth/login
            routes.MapPost("/api/auth/login", async Task<Results<Ok<string>, BadRequest<string>>>(
                [FromBody] LoginRequestDto model,
                SignInManager<AppUser> signInManager,
                UserManager<AppUser> userManager,
                ILogger<Endpoints> logger) =>
            {
                logger.LogInformation("Login attempt for {Email}", model.Email);

                // Find the user by email.
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    logger.LogWarning("Login failed: user with email {Email} not found.", model.Email);
                    return TypedResults.BadRequest("Invalid login credentials.");
                }

                signInManager.AuthenticationScheme = model.UseCookie
                    ? IdentityConstants.ApplicationScheme
                    : IdentityConstants.BearerScheme;

                // Attempt to sign in with the provided password.
                var result = await signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    isPersistent: model.IsPresistCookie,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    logger.LogWarning("Login failed for {Email}: invalid credentials.", model.Email);
                    return TypedResults.BadRequest("Invalid login credentials.");
                }

                logger.LogInformation("User logged in successfully: {Email}", model.Email);
                return TypedResults.Ok("Login successful.");
            });

            // Logout endpoint: POST /api/auth/logout
            routes.MapPost("/api/auth/logout", async (SignInManager<AppUser> signInManager, ILogger<Endpoints> logger) =>
            {
                await signInManager.SignOutAsync();
                logger.LogInformation("User logged out successfully.");
                return Results.Ok(new { message = "Logout successful." });
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

            return routes;
        }
    }
}