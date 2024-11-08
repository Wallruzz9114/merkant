using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Common.EventBus.Integrations;

public class IntegrationEventLog
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private IntegrationEventLog() { }

    public IntegrationEventLog(IntegrationEvent @event, string transactionId)
    {
        EventId = @event.Id;
        CreationTime = @event.CreationDate;
        EventTypeName = @event.GetType().FullName!;
        Content = JsonSerializer.Serialize(@event, @event.GetType(), _jsonSerializerOptions);
        State = EventState.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId;
    }

    public string EventId { get; private set; } = null!;
    public string EventTypeName { get; private set; } = null!;

    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')[^1];

    [NotMapped]
    public IntegrationEvent IntegrationEvent { get; private set; } = null!;

    public EventState State { get; private set; }
    public int TimesSent { get; private set; }
    public DateTimeOffset CreationTime { get; private set; }
    public string Content { get; private set; } = null!;
    public string TransactionId { get; private set; } = null!;

    public void UpdateStatus(EventState status)
    {
        State = status;

        if (status == EventState.InProgress)
            TimesSent++;
    }

    public IntegrationEventLog DeserializeJsonContent(Type type)
    {
        IntegrationEvent = (JsonSerializer.Deserialize(Content, type, _jsonSerializerOptions) as IntegrationEvent)!;
        return this;
    }
}