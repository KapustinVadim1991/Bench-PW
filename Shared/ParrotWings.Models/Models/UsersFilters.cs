namespace ParrotWings.Models.Models;

public class UsersFilters : PaginationFilter
{
    public string? Filter { get; set; }
    public string? SortBy { get; set; } = "name";
    public string? SortOrder { get; set; } = "asc";
}