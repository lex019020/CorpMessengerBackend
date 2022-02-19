using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class UserChatLinkContext : DbContext
    {
        public DbSet<UserChatLink> UserChatLinks { get; set; }

        public UserChatLinkContext(DbContextOptions<UserChatLinkContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
