using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Database.Domain;
using ParrotWings.Models.Dto.Users;
using ParrotWings.Models.Models;

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
            routes.MapGet(
                "/api/users",
                async Task<Results<Ok<GetUsersResponseDto>, UnauthorizedHttpResult>>
                (
                    HttpContext http,
                    UserManager<AppUser> userManager,
                    ILogger<Endpoints> logger,
                    [FromBody] UsersFilters? filters
                ) =>
            {
                filters ??= new UsersFilters();
                logger.LogInformation("GET /api/users called by user: {User}", http.User.Identity?.Name);

                // Only authenticated users have access.
                if (!(http.User.Identity?.IsAuthenticated ?? false))
                {
                    logger.LogWarning("Unauthorized access attempt in GET /api/users.");
                    return TypedResults.Unauthorized();
                }

                var usersQuery = userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(filters.Filter))
                {
                    logger.LogInformation("Applying filter: {filter}", filters.Filter);
                    usersQuery = usersQuery
                        .Where(u =>
                            u.Email!.Contains(filters.Filter) ||
                            u.FullName.Contains(filters.Filter)
                        );
                }

                // Sorting (by email or full name)
                logger.LogInformation("Sorting users by {SortBy} in {SortOrder} order", filters.SortBy, filters.SortOrder);
                usersQuery = filters.SortBy?.ToLower() switch
                {
                    "fullname" => filters.SortOrder?.ToLower() == "asc"
                                    ? usersQuery.OrderBy(u => u.FullName)
                                    : usersQuery.OrderByDescending(u => u.FullName),
                    _ => filters.SortOrder?.ToLower() == "asc"
                                    ? usersQuery.OrderBy(u => u.Email)
                                    : usersQuery.OrderByDescending(u => u.Email)
                };

                // Apply pagination.
                logger.LogInformation("Applying pagination: startIndex = {StartIndex}, count = {Count}", filters.StartIndex, filters.Count);
                var users = await usersQuery.Skip(filters.StartIndex).Take(filters.Count).ToListAsync();
                logger.LogInformation("Found {Count} users", users.Count);

                // Return simplified user data.
                var result = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email!,
                    FullName = u.FullName
                });
                return TypedResults.Ok(new GetUsersResponseDto
                {
                    Users = result,
                });
            })
            .RequireAuthorization();

            // POST /api/user/balance – get user balance.
            routes.MapGet("/api/user/balance", async Task<Results<Ok<decimal>, UnauthorizedHttpResult>> (
                HttpContext http,
                UserManager<AppUser> userManager,
                ILogger<Endpoints> logger) =>
            {
                logger.LogInformation("POST /api/user/balance called by user: {User}", http.User.Identity?.Name);

                var email = http.User.FindFirst(ClaimTypes.Email)?.Value;
                if (http.User.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(email))
                {
                    logger.LogWarning("User email is null or empty in POST /api/user/balance.");
                    return TypedResults.Unauthorized();
                }


                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    logger.LogWarning("User not found for email: {UserEmail} in POST /api/user/balance.", email);
                    return TypedResults.Unauthorized();
                }

                logger.LogInformation("User {UserEmail} balance retrieved: {Balance}", email, user.Balance);
                return TypedResults.Ok(user.Balance);
            })
            .RequireAuthorization();

            return routes;
        }
    }
}