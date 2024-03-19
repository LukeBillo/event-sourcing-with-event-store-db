namespace EventSourcing.EventStoreDB.Common;

public interface IEventStore
{
    Task<IEnumerable<Event>> GetEvents(string streamName);
    Task<WriteResult> WriteEvents(string streamName, IEnumerable<Event> events);
}
