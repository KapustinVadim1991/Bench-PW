using ParrotWings.Models.Models;

namespace ParrotWings.Models.Dto.Transactions.GetTransactions;

public class GetTransactionsRequestDto
{
    public TransactionsFilters Filters { get; set; } = new();
}