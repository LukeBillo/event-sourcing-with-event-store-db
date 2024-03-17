using Microsoft.EntityFrameworkCore;

namespace EventSourcing.WithoutAggregates.API.Data.Sql;

public class BankAccountContext : DbContext
{
    public DbSet<BankAccount> BankAccounts { get; set; }

    public BankAccountContext(DbContextOptions<BankAccountContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>().ToTable(BankAccount.Table);
    }
}
