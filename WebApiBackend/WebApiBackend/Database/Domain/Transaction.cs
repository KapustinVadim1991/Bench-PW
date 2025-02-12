using System.ComponentModel.DataAnnotations;

namespace WebApiBackend.Database.Domain;

/// <summary>
/// Represents a transaction between two users.
/// </summary>
public class Transaction
{
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The date and time when the transaction was made.
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string SenderId { get; set; } = null!;

    public AppUser Sender { get; set; } = null!;

    [Required]
    public string RecipientId { get; set; } = null!;

    public AppUser Recipient { get; set; } = null!;

    /// <summary>
    /// The amount of the transaction (a positive value).
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}