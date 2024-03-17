using Microsoft.EntityFrameworkCore;

namespace EventSourcing.WithoutAggregates.API.Data.Sql;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBankAccountsSql(this IServiceCollection services)
    {
        return services.AddDbContext<BankAccountContext>(
            options => options.UseSqlServer("Server=127.0.0.1;Database=main;User Id=sa;Password=MyDefaultPassword!;TrustServerCertificate=true"));
    }
}
