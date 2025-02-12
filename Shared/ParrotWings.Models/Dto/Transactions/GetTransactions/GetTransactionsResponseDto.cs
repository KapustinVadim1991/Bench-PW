namespace ParrotWings.Models.Dto.Transactions.GetTransactions;

public class GetTransactionsResponseDto
{
    public IEnumerable<TransactionDto> Transactions { get; set; } = [];
}