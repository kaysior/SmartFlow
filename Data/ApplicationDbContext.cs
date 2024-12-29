using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Models;

namespace SmartFlow.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SavingsGoal> SavingsGoal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja relacji SavingsGoal -> Transactions
            modelBuilder.Entity<SavingsGoal>()
                .HasMany<Transaction>()
                .WithOne(t => t.SavingsGoal)
                .HasForeignKey(t => t.SavingsGoalId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
