using System.ComponentModel.DataAnnotations;

namespace ParrotWings.Models.Dto.Transactions.CreateTransaction;

/// <summary>
/// Model for creating a new transaction (transfer).
/// </summary>
public class CreateTransactionRequestDto
{
    [Required, EmailAddress] public string RecipientEmail { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}