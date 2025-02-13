using Client.Identity.Models;
using ParrotWings.Models.Dto.Users;
using ParrotWings.Models.Models;

namespace Client.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Calls the GET /api/users endpoint with optional filters.
    /// Note: Since GET requests with a body are not standard, an HttpRequestMessage with a GET method and a JSON body is used.
    /// </summary>
    Task<RequestResult<IReadOnlyList<UserInfo>>> GetUsersAsync(UsersFilters? filters);

    /// <summary>
    /// Calls the POST /api/user/balance endpoint to retrieve the current user's balance.
    /// </summary>
    Task<RequestResult<decimal>> GetUserBalanceAsync();
}