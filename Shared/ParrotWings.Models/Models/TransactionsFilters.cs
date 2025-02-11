namespace ParrotWings.Models.Dto.Transactions;

public class TransactionsFilters
{
    public int StartIndex { get; set; }
    public int Count { get; set; }
    public string? Filter { get; set; }
}