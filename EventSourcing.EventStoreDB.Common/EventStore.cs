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
        return await GetVersionedEvents(streamName);
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

    public async Task<IEnumerable<VersionedEvent>> GetVersionedEvents(string streamName,
        long upToVersion = Int64.MaxValue)
    {
        var readStreamResult = _eventStoreClient.ReadStreamAsync(
            Direction.Forwards,
            streamName,
            StreamPosition.Start,
            upToVersion);

        if (await readStreamResult.ReadState is ReadState.StreamNotFound)
        {
            return new List<VersionedEvent>();
        }

        return await readStreamResult.Select(DeserializeEvent).ToListAsync();
    }

    public async Task<VersionedWriteResult> WriteVersionedEvents(string streamName, IEnumerable<VersionedEvent> events)
    {
        var latestEvent = await GetLatestEvent(streamName);

        var revisionForStream =
            latestEvent is not null ? StreamRevision.FromInt64(latestEvent.Version) : StreamRevision.None;

        var eventsData = events.Select(ToEventData).ToArray();

        try
        {
            var result = await _eventStoreClient.AppendToStreamAsync(streamName, revisionForStream, eventsData);
            return new VersionedWriteResult(true, result.NextExpectedStreamRevision.ToInt64());
        }
        catch (Exception e) when (IsInvalidOrWrongVersion(e))
        {
            return new VersionedWriteResult(false, revisionForStream.ToInt64());
        }
    }

    private async ValueTask<VersionedEvent?> GetLatestEvent(string streamName)
    {
        var readStreamResult = _eventStoreClient.ReadStreamAsync(
            Direction.Backwards,
            streamName,
            StreamPosition.End,
            1);

        if (await readStreamResult.ReadState == ReadState.StreamNotFound)
        {
            return null;
        }

        var @event = await readStreamResult.SingleAsync();
        return DeserializeEvent(@event);
    }

    private VersionedEvent DeserializeEvent(ResolvedEvent @event)
    {
        var type = Type.GetType(@event.OriginalEvent.EventType);
        if (type is null)
        {
            throw new InvalidOperationException($"Event type {@event.OriginalEvent.EventType} not found");
        }

        var deserializedEvent = JsonSerializer.Deserialize(@event.OriginalEvent.Data.Span, type);
        if (deserializedEvent is not VersionedEvent versionedEvent)
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
        return new EventData(Uuid.NewUuid(), @event.GetType().AssemblyQualifiedName!, bytes);
    }

    private static bool IsInvalidOrWrongVersion(Exception e) =>
        e is InvalidOperationException or WrongExpectedVersionException;
}
