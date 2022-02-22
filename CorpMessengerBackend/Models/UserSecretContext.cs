using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class UserSecretContext : DbContext
    {
        public DbSet<UserSecret> UserSecrets { get; set; }

        public UserSecretContext(DbContextOptions<UserSecretContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
