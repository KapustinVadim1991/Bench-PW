using System.Diagnostics.CodeAnalysis;

namespace Client.Identity.Models;

/// <summary>
/// User info from identity endpoint to establish claims.
/// </summary>

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = [];
}