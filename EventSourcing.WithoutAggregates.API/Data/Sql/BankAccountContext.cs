using Microsoft.EntityFrameworkCore;

namespace EventSourcing.WithoutAggregates.API.Data.Sql;

public sealed class BankAccountContext : DbContext
{
    public DbSet<BankAccount> BankAccounts { get; set; }

    public BankAccountContext(DbContextOptions<BankAccountContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
