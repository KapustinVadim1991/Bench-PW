using System.ComponentModel.DataAnnotations;

namespace ParrotWings.Models.Dto.Transactions;

/// <summary>
/// Model for creating a new transaction (transfer).
/// </summary>
public class CreateTransactionRequestDto
{
    public string RecipientId { get; set; } = null!;
    public decimal Amount { get; set; }
}