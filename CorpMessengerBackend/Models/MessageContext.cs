using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class MessageContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public MessageContext(DbContextOptions<MessageContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
