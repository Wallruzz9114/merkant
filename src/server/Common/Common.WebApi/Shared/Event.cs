using System.Text.Json;

namespace Common.WebApi.Shared;

public abstract class Event
{
    public string MessageType { get; protected set; }
    public string AggregateId { get; protected set; }
    public DateTimeOffset CreatedAt { get; set; }

    protected Event(string aggregateId)
    {
        MessageType = GetType().Name;
        AggregateId = aggregateId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}