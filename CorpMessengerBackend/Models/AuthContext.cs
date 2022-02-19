using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class AuthContext : DbContext
    {
        public DbSet<Auth> Auths { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
