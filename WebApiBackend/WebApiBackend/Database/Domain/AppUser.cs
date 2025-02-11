using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiBackend.Database.Domain;

public class AppUser : IdentityUser
{
    [PersonalData]
    [MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; }
}