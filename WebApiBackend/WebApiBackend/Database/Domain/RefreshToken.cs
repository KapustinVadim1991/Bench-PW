using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiBackend.Database.Domain;

public class RefreshToken
{
    [Key] public int Id { get; set; }

    [Required] public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public string CreatedByIp { get; set; } = string.Empty;
    public DateTime? Revoked { get; set; }

    public string? RevokedByIp { get; set; }

    [NotMapped] public bool IsExpired => DateTime.UtcNow >= Expires;

    [NotMapped] public bool IsActive => Revoked == null && !IsExpired;

    [Required] public string UserId { get; set; } = null!;

    public AppUser User { get; set; } = null!;
}