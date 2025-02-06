using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiBackend.Identity;

public class AppUser : IdentityUser
{
    [PersonalData]
    [MaxLength(100)]
    public string FullName { get; set; } = null!;
}