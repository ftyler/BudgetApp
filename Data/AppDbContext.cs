using Microsoft.EntityFrameworkCore;
using BudgetApp.Models;

namespace BudgetApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<BudgetSetting> BudgetSettings => Set<BudgetSetting>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Category>().Property(c => c.MonthlyLimit).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasColumnType("decimal(18,2)");

            // Seed sample data
            modelBuilder.Entity<BudgetSetting>().HasData(new BudgetSetting { Id = 1, MonthlyBudget = 3000m });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Groceries", MonthlyLimit = 400m },
                new Category { Id = 2, Name = "Rent", MonthlyLimit = 1200m },
                new Category { Id = 3, Name = "Restaurants", MonthlyLimit = 300m }
            );

            modelBuilder.Entity<Transaction>().HasData(
                new Transaction { Id = 1, Date = DateTime.Today, Payee = "Supermarket", Amount = 75m, CategoryId = 1, Notes = "Weekly groceries" },
                new Transaction { Id = 2, Date = DateTime.Today.AddDays(-1), Payee = "Landlord", Amount = 1200m, CategoryId = 2, Notes = "Monthly rent" }
            );
        }
    }
}
