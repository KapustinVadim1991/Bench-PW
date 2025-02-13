using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Database.Domain;
using ParrotWings.Models.Dto.Users;

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
            routes.MapGet("/api/users", async Task<Results<Ok<GetUsersResponseDto>, UnauthorizedHttpResult>> (
                    HttpContext http,
                    UserManager<AppUser> userManager,
                    ILogger<Endpoints> logger,
                    [FromQuery] string? filter,
                    [FromQuery] int startIndex,
                    [FromQuery] int count,
                    [FromQuery] string sortBy = "name",
                    [FromQuery] string sortOrder = "asc"
                    ) =>
                {
                    logger.LogInformation("GET /api/users called by user: {User}", http.User.Identity?.Name);

                    if (!(http.User.Identity?.IsAuthenticated ?? false))
                    {
                        logger.LogWarning("Unauthorized access attempt in GET /api/users.");
                        return TypedResults.Unauthorized();
                    }

                    var usersQuery = userManager.Users.AsQueryable();

                    if (!string.IsNullOrEmpty(filter))
                    {
                        logger.LogInformation("Applying filter: {filter}", filter);
                        usersQuery = usersQuery.Where(u =>
                            u.Email!.Contains(filter) ||
                            u.FullName.Contains(filter));
                    }

                    // Sorting
                    logger.LogInformation("Sorting users by {SortBy} in {SortOrder} order", sortBy, sortOrder);
                    usersQuery = sortBy.ToLower() switch
                    {
                        "fullname" => sortOrder.ToLower() == "asc"
                            ? usersQuery.OrderBy(u => u.FullName)
                            : usersQuery.OrderByDescending(u => u.FullName),
                        _ => sortOrder.ToLower() == "asc"
                            ? usersQuery.OrderBy(u => u.Email)
                            : usersQuery.OrderByDescending(u => u.Email)
                    };

                    // Pagination
                    logger.LogInformation("Applying pagination: startIndex = {StartIndex}, count = {Count}", startIndex, count);
                    var totalCount = usersQuery.Count();
                    var users = await usersQuery.Skip(startIndex).Take(count).ToListAsync();
                    logger.LogInformation("Found {Count} users", users.Count);

                    var result = users.Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email!,
                        FullName = u.FullName
                    });

                    return TypedResults.Ok(new GetUsersResponseDto { Users = result, TotalCount = totalCount });
                })
                .RequireAuthorization();

            // POST /api/user/balance – get user balance.
            routes.MapGet("/api/user/balance", async Task<Results<Ok<decimal>, UnauthorizedHttpResult>> (
                HttpContext http,
                UserManager<AppUser> userManager,
                ILogger<Endpoints> logger) =>
            {

                var email = http.User.FindFirst(ClaimTypes.Email)?.Value;
                logger.LogInformation("POST /api/user/balance called by user: {User}", email);

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