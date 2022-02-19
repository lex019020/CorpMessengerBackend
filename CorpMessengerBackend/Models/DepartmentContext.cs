using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public sealed class DepartmentContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }

        public DepartmentContext(DbContextOptions<DepartmentContext> options)
            :base(options)
        {
            Database.EnsureCreated();
        }
    }
}
