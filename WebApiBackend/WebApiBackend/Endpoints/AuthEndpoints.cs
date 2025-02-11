using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ParrotWings.Models.Dto.Identity;
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
                async Task<Results<Ok<string>, BadRequest<string>, BadRequest<ValidationProblem>>> (
                    RegisterRequestDto model,
                    UserManager<AppUser> userManager,
                    SignInManager<AppUser> signInManager
            ) =>
            {
                // Check if a user with the provided email already exists.
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
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
                    return TypedResults.BadRequest(result.CreateValidationProblem());
                }

                // Sign in the user (set authentication cookies)
                await signInManager.SignInAsync(user, isPersistent: false);
                return TypedResults.Ok("Registration successful.");
            });

            // Login endpoint: POST /api/auth/login
            routes.MapPost("/api/auth/login", async Task<Results<Ok<string>, BadRequest<string>>> (
                LoginRequestDto model,
                SignInManager<AppUser> signInManager,
                UserManager<AppUser> userManager) =>
            {
                // Find the user by email.
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return TypedResults.BadRequest("Invalid login credentials.");
                }

                signInManager.AuthenticationScheme = model.UseCookie ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

                // Attempt to sign in with the provided password.
                var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.IsPresistCookie, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return TypedResults.BadRequest("Invalid login credentials.");
                }

                return TypedResults.Ok("Login successful.");
            });

            // Logout endpoint: POST /api/auth/logout
            routes.MapPost("/api/auth/logout", async (SignInManager<AppUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok(new { message = "Logout successful." });
            });

            routes.MapGet("/api/auth/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
                (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
            {
                var userManager = sp.GetRequiredService<UserManager<AppUser>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(new InfoResponse {Email = user.Email!, IsEmailConfirmed = false});
            });

            return routes;
        }
    }
}