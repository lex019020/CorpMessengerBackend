using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
