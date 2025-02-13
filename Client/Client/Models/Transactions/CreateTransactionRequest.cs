using System.ComponentModel.DataAnnotations;

namespace Client.Models.Transactions;

/// <summary>
/// Model for creating a new transaction (transfer).
/// </summary>
public class CreateTransactionRequest
{
    [Required, EmailAddress] public string RecipientEmail { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}