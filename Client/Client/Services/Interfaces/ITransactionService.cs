using Client.Identity.Models;
using Client.Models.Transactions;
using ParrotWings.Models.Models;

namespace Client.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<RequestResult<GetTransactionsResponse>> GetTransactionsAsync(TransactionsFilters filters);

        Task<RequestResult> CreateTransactionAsync(CreateTransactionRequest model);
    }
}