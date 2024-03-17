using EventSourcing.EventStoreDB.Common;

namespace EventSourcing.Aggregates.API.Data.EventStore;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEventStoreDB(this IServiceCollection services)
    {
        return services
            .AddEventStoreClient("esdb://localhost:2114?tls=false&tlsVerifyCert=false")
            .AddSingleton<IEventStore, EventStoreDB.Common.EventStore>();
    }
}
