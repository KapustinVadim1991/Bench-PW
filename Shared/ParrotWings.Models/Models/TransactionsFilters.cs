namespace ParrotWings.Models.Models;

public class TransactionsFilters : PaginationFilter
{
    public string? Filter { get; set; }
    public string? SortBy { get; set; } = "date";
    public string? SortOrder { get; set; } = "asc";
}