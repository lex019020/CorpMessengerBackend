using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models;

public sealed class AppDataContext : DbContext, IAppDataContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Auth> Auths { get; set; }
    public DbSet<AdminAuth> AdminAuths { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<UserChatLink> UserChatLinks { get; set; }
    public DbSet<UserSecret> UserSecrets { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserSecret>()
            .HasIndex(u => u.UserId)
            .IsUnique();
    }
}