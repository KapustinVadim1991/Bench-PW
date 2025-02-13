using System.Net.Http.Json;
using Client.Helper;
using Client.Identity.Models;
using Client.Models.Transactions;
using Client.Models.Users;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using ParrotWings.Models.Dto.Transactions.GetTransactions;
using ParrotWings.Models.Models;

namespace Client.Services
{
    /// <summary>
    /// Implementation of the transaction service for interacting with the transaction API endpoints.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TransactionService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionService"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="logger">The logger instance.</param>
        public TransactionService(IHttpClientFactory httpClientFactory, ILogger<TransactionService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of transactions with filtering, sorting, and pagination.
        /// Calls the GET /api/transactions endpoint.
        /// </summary>
        /// <param name="filters">The filtering, sorting, and pagination parameters.</param>
        /// <returns>A <see cref="RequestResult"/> containing <see cref="GetTransactionsResponse"/> data.</returns>
        public async Task<RequestResult<GetTransactionsResponse>> GetTransactionsAsync(TransactionsFilters filters)
        {
            try
            {
                // Prepare query parameters.
                var queryParams = new Dictionary<string, string>
                {
                    { "startIndex", filters.StartIndex.ToString() },
                    { "count", filters.Count.ToString() }
                };

                if (!string.IsNullOrWhiteSpace(filters.Filter))
                {
                    queryParams.Add("filter", filters.Filter);
                }

                if (!string.IsNullOrWhiteSpace(filters.SortBy))
                {
                    queryParams.Add("sortBy", filters.SortBy);
                }

                if (!string.IsNullOrWhiteSpace(filters.SortOrder))
                {
                    queryParams.Add("sortOrder", filters.SortOrder);
                }

                // Build the URL with query string parameters.
                var url = QueryHelpers.AddQueryString("/api/transactions", queryParams!);
                _logger.LogInformation("Executing GET request: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<GetTransactionsResponseDto>(
                        GetTransactionsResponseDtoJsonContext.Default.GetTransactionsResponseDto);

                    // Map the DTO to the client-side model.
                    var result = new GetTransactionsResponse
                    {
                        TotalCount = dto?.TotalCount ?? 0,
                        Transactions = dto?.Transactions.Select(t => new Transaction
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            TransactionDate = t.TransactionDate,
                            Sender = new UserInfo(t.Sender.Id, t.Sender.Email, t.Sender.FullName),
                            Recipient = new UserInfo(t.Recipient.Id, t.Recipient.Email, t.Recipient.FullName),
                        }).ToList() ?? []
                    };

                    return new RequestResult<GetTransactionsResponse>
                    {
                        Succeeded = true,
                        Data = result
                    };
                }

                _logger.LogWarning("GetTransactionsAsync returned status code: {StatusCode}", response.StatusCode);
                return new RequestResult<GetTransactionsResponse>
                {
                    Succeeded = false,
                    ErrorList = [$"Error retrieving transactions. Status code: {response.StatusCode}" ]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetTransactionsAsync");
                return new RequestResult<GetTransactionsResponse>
                {
                    Succeeded = false,
                    ErrorList = ["An unexpected error occurred while retrieving transactions." ]
                };
            }
        }

        /// <summary>
        /// Creates a new transaction.
        /// Calls the POST /api/transaction endpoint.
        /// </summary>
        /// <param name="model">The transaction creation request model.</param>
        /// <returns>A <see cref="RequestResult{T}"/> containing a success message or error details.</returns>
        public async Task<RequestResult> CreateTransactionAsync(CreateTransactionRequest model)
        {
            try
            {
                _logger.LogInformation("Sending request to create a transaction.");
                var response = await _httpClient.PostAsJsonAsync("/api/transaction",
                    model, CreateTransactionRequestJsonContext.Default.CreateTransactionRequest);

                if (response.IsSuccessStatusCode)
                {
                    return new RequestResult { Succeeded = true };
                }

                _logger.LogWarning("CreateTransactionAsync returned status code: {StatusCode}", response.StatusCode);
                return new RequestResult
                {
                    Succeeded = false,
                    ErrorList = [$"Error creating transaction. Status code: {response.StatusCode}"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in CreateTransactionAsync");
                return new RequestResult
                {
                    Succeeded = false,
                    ErrorList = ["An unexpected error occurred while creating the transaction."]
                };
            }
        }
    }
}