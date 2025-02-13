using System.Net.Http.Json;
using Client.Helper;
using Client.Identity.Models;
using Client.Models.Users;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using ParrotWings.Models.Dto.Users;
using ParrotWings.Models.Models;

namespace Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClientFactory httpClientFactory, ILogger<UserService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        /// <summary>
        /// Calls the GET /api/users endpoint with optional filters.
        /// Note: Since GET requests with a body are not standard, an HttpRequestMessage with a GET method and a JSON body is used.
        /// </summary>
        public async Task<RequestResult<GetUsersResponse>> GetUsersAsync(UsersFilters filters)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (!string.IsNullOrWhiteSpace(filters.Filter))
                    queryParams.Add("filter", filters.Filter);

                if (!string.IsNullOrWhiteSpace(filters.SortBy))
                    queryParams.Add("sortBy", filters.SortBy);

                if (!string.IsNullOrWhiteSpace(filters.SortOrder))
                    queryParams.Add("sortOrder", filters.SortOrder);

                queryParams.Add("startIndex", filters.StartIndex.ToString());
                queryParams.Add("count", filters.Count.ToString());

                var url = QueryHelpers.AddQueryString("/api/users", queryParams!);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GetUsersResponseDto>(
                        GetUsersResponseDtoJsonContext.Default.GetUsersResponseDto);

                    return new RequestResult<GetUsersResponse>
                    {
                        Succeeded = true,
                        Data = new GetUsersResponse
                        {
                            TotalCount = result?.TotalCount ?? 0,
                            Users = result?.Users.Select(x=> new UserInfo(x.Id, x.Email, x.FullName)).ToList() ?? []
                        }
                    };
                }

                _logger.LogWarning("GetUsersAsync failed with status code: {StatusCode}", response.StatusCode);
                return new RequestResult<GetUsersResponse> { Succeeded = true, ErrorList = [ $"An error occurred during the users retrieval. Status is {response.StatusCode}"]};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetUsersAsync");
                return new RequestResult<GetUsersResponse> { Succeeded = true, ErrorList = [ "An unexpected error occurred during the users retrieval."] };
            }
        }

        /// <summary>
        /// Calls the POST /api/user/balance endpoint to retrieve the current user's balance.
        /// </summary>
        public async Task<RequestResult<decimal>> GetUserBalanceAsync()
        {
            try
            {
                // Send a POST request without a body (using an empty StringContent)

                var response = await _httpClient.GetAsync("/api/user/balance");
                if (response.IsSuccessStatusCode)
                {
                    var responseBalance = await response.Content.ReadAsStringAsync();
                    var balance = decimal.Parse(responseBalance);
                    return new RequestResult<decimal> { Succeeded = true, Data = balance };
                }

                _logger.LogWarning("GetUserBalanceAsync failed with status code: {StatusCode}", response.StatusCode);
                return new RequestResult<decimal> { Succeeded = false, ErrorList = [ $"An error occurred during the balance retrieval. Status is {response.StatusCode}"] };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetUserBalanceAsync");
                return new RequestResult<decimal> { Succeeded = false, ErrorList = [ "An unexpected error occurred during the balance retrieval."] };
            }
        }
    }
}