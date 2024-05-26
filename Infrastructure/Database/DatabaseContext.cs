using Domain.Entities.Account;
using Domain.Entities.Transaction;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(p => p.AccountNumber)
            .IsUnique();
        
        modelBuilder.Entity<Account>()
            .HasIndex(p => p.Email)
            .IsUnique();
        
        modelBuilder.Entity<Account>()
            .HasIndex(p => p.PhoneNumber)
            .IsUnique();
    }
}