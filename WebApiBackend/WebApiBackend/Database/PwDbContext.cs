using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApiBackend.Database;

public class PwDbContext : IdentityDbContext<IdentityUser>
{
    public PwDbContext(DbContextOptions<PwDbContext> options) :
        base(options)
    { }
}