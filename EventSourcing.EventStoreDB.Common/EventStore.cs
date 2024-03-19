using System.Reflection;
using System.Text;
using EventStore.Client;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EventSourcing.EventStoreDB.Common;

public class EventStore : IEventStore
{
    private readonly EventStoreClient _eventStoreClient;

    public EventStore(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task<IEnumerable<Event>> GetEvents(string streamName)
    {
        var readStreamResult = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start);

        if (await readStreamResult.ReadState is ReadState.StreamNotFound)
        {
            return new List<Event>();
        }

        return await readStreamResult.Select(DeserializeEvent).ToListAsync();
    }

    public async Task<WriteResult> WriteEvents(string streamName, IEnumerable<Event> events)
    {
        try
        {
            var eventsData = events.Select(ToEventData).ToArray();
            await _eventStoreClient.AppendToStreamAsync(streamName, StreamState.Any, eventsData);

            return new WriteResult(true);
        }
        catch (Exception e) when (IsInvalidOrWrongVersion(e))
        {
            return new WriteResult(false);
        }
    }

    private Event DeserializeEvent(ResolvedEvent @event)
    {
        var type = Assembly.GetEntryAssembly()!.GetType(@event.OriginalEvent.EventType);
        if (type is null)
        {
            throw new InvalidOperationException($"Event type {@event.OriginalEvent.EventType} not found");
        }

        var deserializedEvent = JsonSerializer.Deserialize(@event.OriginalEvent.Data.Span, type);
        if (deserializedEvent is not Event versionedEvent)
        {
            throw new InvalidOperationException(
                $"Deserialized event {@event.OriginalEvent.EventId.ToString()} could not be deserialized to {nameof(VersionedEvent)}");
        }

        return versionedEvent;
    }

    private static EventData ToEventData<TEvent>(TEvent @event) where TEvent : Event
    {
        var json = JsonConvert.SerializeObject(@event);
        var bytes = Encoding.UTF8.GetBytes(json);
        return new EventData(Uuid.NewUuid(), @event.GetType().FullName!, bytes);
    }

    private static bool IsInvalidOrWrongVersion(Exception e) =>
        e is InvalidOperationException or WrongExpectedVersionException;
}
