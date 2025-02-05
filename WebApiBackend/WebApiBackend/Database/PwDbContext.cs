using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Identity;

namespace WebApiBackend.Database;

public class PwDbContext : IdentityDbContext<AppUser>
{
    public PwDbContext(DbContextOptions<PwDbContext> options) :
        base(options)
    { }
}