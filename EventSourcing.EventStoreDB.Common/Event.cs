namespace EventSourcing.EventStoreDB.Common;

public record Event
{
    public required DateTime Timestamp { get; init; }
}
