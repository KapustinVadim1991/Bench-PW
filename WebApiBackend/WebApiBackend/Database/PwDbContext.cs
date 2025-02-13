using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Database.Domain;

namespace WebApiBackend.Database;

public class PwDbContext : IdentityDbContext<AppUser>
{
    public PwDbContext(DbContextOptions<PwDbContext> options) :
        base(options)
    { }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Transaction>(entity =>
        {
            entity.HasOne(t => t.Sender)
                .WithMany(u => u.TransactionsSent)
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Recipient)
                .WithMany(u => u.TransactionsReceived)
                .HasForeignKey(t => t.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}