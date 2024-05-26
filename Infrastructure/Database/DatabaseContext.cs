using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Account> Accounts { get; set; }
    
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