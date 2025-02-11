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
}