using Microsoft.EntityFrameworkCore;
using Bangk.Repositories.Entities;

namespace Bangk.Repositories.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>().Ignore(_ => _.CreationDate);
            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
