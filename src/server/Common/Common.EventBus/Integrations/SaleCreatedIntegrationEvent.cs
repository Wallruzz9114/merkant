using System.Text.Json.Serialization;

namespace Common.EventBus.Integrations;

public record SaleCreatedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; private init; }
    public Dictionary<string, int> Itens { get; private init; }

    public SaleCreatedIntegrationEvent(string userId, Dictionary<string, int> itens)
    {
        UserId = userId;
        Itens = itens;
    }

    [JsonConstructor]
    public SaleCreatedIntegrationEvent(string userId, Dictionary<string, int> itens, string id, DateTimeOffset creationDate, string? key) : base(id, creationDate, key)
    {
        UserId = userId;
        Itens = itens;
    }
}