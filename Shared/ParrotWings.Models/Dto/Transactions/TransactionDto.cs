using ParrotWings.Models.Dto.Users;

namespace ParrotWings.Models.Dto.Transactions;

public class TransactionDto
{
    public Guid Id { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public UserDto Sender { get; set; } = null!;
    public UserDto Recipient { get; set; } = null!;
    public decimal Amount { get; set; }
}