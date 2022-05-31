using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CorpMessengerBackend.Models;

public interface IAppDataContext
{
    DbSet<Auth> Auths { get; set; }
    DbSet<AdminAuth> AdminAuths { get; set; }
    DbSet<Chat> Chats { get; set; }
    DbSet<Department> Departments { get; set; }
    DbSet<Message> Messages { get; set; }
    DbSet<UserChatLink> UserChatLinks { get; set; }
    DbSet<UserSecret> UserSecrets { get; set; }
    DbSet<User> Users { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry Add(object entity);
    ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken);
    EntityEntry Update(object entity);
}