using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class ChatContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }

        public ChatContext(DbContextOptions<ChatContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
