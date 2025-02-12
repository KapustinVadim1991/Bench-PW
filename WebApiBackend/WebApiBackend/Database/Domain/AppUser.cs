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

    public ICollection<Transaction> TransactionsSent { get; set; } = new List<Transaction>();

    public ICollection<Transaction> TransactionsReceived { get; set; } = new List<Transaction>();
}