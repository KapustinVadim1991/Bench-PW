using Client.Models.Users;

namespace Client.Models.Transactions;

public class Transaction
{
    public Guid Id { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public UserInfo Sender { get; set; } = null!;
    public UserInfo Recipient { get; set; } = null!;
    public decimal Amount { get; set; }
}