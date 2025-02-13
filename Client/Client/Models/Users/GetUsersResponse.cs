namespace Client.Models.Users;

public class GetUsersResponse
{
    public IReadOnlyList<UserInfo> Users { get; set; } = [];

    public int TotalCount { get; set; }
}