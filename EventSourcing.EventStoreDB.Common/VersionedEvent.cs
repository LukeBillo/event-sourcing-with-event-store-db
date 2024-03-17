namespace EventSourcing.EventStoreDB.Common;

public record VersionedEvent : Event
{
    public required long Version { get; init; }
}
