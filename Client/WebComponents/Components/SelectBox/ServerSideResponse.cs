namespace WebComponents.Components.SelectBox;

public class ServerSideResponse<TItem>
{
    public int TotalCount { get; set; }
    public IEnumerable<TItem> Items { get; set; } = [];
}