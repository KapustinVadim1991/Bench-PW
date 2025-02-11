using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Database.Domain;

namespace WebApiBackend.Endpoints
{
    /// <summary>
    /// Contains endpoints for handling users.
    /// </summary>
    public static class UserEndpoints
    {
        public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            // GET /api/users – retrieves a list of users with pagination, filtering by email and/or full name, and sorting.
            routes.MapGet("/api/users", async (
                    HttpContext http,
                    UserManager<AppUser> userManager,
                    int startIndex = 0, int count = 10,
                    string? sortBy = "email",
                    string? sortOrder = "asc",
                    string? emailFilter = null,
                    string? fullNameFilter = null) =>
            {
                // Only authenticated users have access.
                if (!http.User.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }

                var usersQuery = userManager.Users.AsQueryable();

                // Filter by email if provided.
                if (!string.IsNullOrEmpty(emailFilter))
                {
                    usersQuery = usersQuery.Where(u => u.Email!.Contains(emailFilter));
                }
                // Filter by full name if provided.
                if (!string.IsNullOrEmpty(fullNameFilter))
                {
                    usersQuery = usersQuery.Where(u => u.FullName.Contains(fullNameFilter));
                }

                // Sorting (by email or full name)
                usersQuery = sortBy?.ToLower() switch
                {
                    "fullname" => sortOrder?.ToLower() == "asc" ? usersQuery.OrderBy(u => u.FullName) : usersQuery.OrderByDescending(u => u.FullName),
                    _ => sortOrder?.ToLower() == "asc" ? usersQuery.OrderBy(u => u.Email) : usersQuery.OrderByDescending(u => u.Email)
                };

                // Apply pagination.
                var users = await usersQuery.Skip(startIndex).Take(count).ToListAsync();

                // Return simplified user data.
                var result = users.Select(u => new { u.Id, u.FullName, u.Email, u.Balance });
                return Results.Ok(result);
            })
            .RequireAuthorization();

            // POST /api/user/balance – get user balance.
            routes.MapPost("/api/user/balance", async (
                    HttpContext http,
                    UserManager<AppUser> userManager
                ) =>
            {
                // Retrieve the current user (sender) based on the authentication cookie.
                var userEmail = http.User.Identity?.Name;
                if (userEmail == null)
                {
                    return Results.Unauthorized();
                }

                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(new { user.Balance });
            })
            .RequireAuthorization();

            return routes;
        }
    }
}