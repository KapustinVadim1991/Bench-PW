namespace Client.Models.Transactions;

public class GetTransactionsResponse
{
    public IReadOnlyList<Transaction> Transactions { get; set; } = [];

    public int TotalCount { get; set; }
}