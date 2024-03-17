namespace EventSourcing.EventStoreDB.Common;

public interface IEventStore
{
    Task<IEnumerable<Event>> GetEvents(string streamName);
    Task<WriteResult> WriteEvents(string streamName, IEnumerable<Event> events);
    Task<IEnumerable<VersionedEvent>> GetVersionedEvents(string streamName, long upToVersion = long.MaxValue);
    Task<VersionedWriteResult> WriteVersionedEvents(string streamName, IEnumerable<VersionedEvent> events);
}
