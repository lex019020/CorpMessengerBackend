using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class AppDataContext : DbContext
    {
        public DbSet<Auth> Auths { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserChatLink> UserChatLinks { get; set; }
        public DbSet<UserSecret> UserSecrets { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
