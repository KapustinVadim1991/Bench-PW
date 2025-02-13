namespace ParrotWings.Models.Dto.Transactions.GetTransactions;

public class GetTransactionsResponseDto
{
    public IEnumerable<TransactionDto> Transactions { get; set; } = [];
    public int TotalCount { get; set; }
}