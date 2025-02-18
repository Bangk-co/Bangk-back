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
            modelBuilder.Entity<Setting>().Ignore(_ => _.CreationDate);
            modelBuilder.Entity<Notification>().Ignore(_ => _.CreationDate);
            modelBuilder.Entity<ScheduleIntegration>().Ignore(_ => _.CreationDate); 
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<GroupSetting> GroupSettings { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<LoanApplication> LoanApplication { get; set; }
        public DbSet<LoanApplicationDocument> LoanApplicationDocument { get; set; }
        public DbSet<LoanApplicationStateHistory> LoanApplicationStateHistory { get; set; }
        public DbSet<LoanQuote> LoanQuote { get; set; }
        public DbSet<LoanInsurancePolicy> LoanInsurancePolicy { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<LoanForm> LoanForms { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ValidationSBSLog> ValidationSBSLogs { get; set; }
        public DbSet<ScheduleIntegration> ScheduleIntegrations { get; set; }
        public DbSet<ValidationSarlaftLog> ValidationSarlaftLogs { get; set; }
        public DbSet<MedPersonInsureds> MedPersonInsureds { get; set; }
        public DbSet<MedEnsuranceApplication> MedEnsuranceApplication { get; set; }

    }
}
