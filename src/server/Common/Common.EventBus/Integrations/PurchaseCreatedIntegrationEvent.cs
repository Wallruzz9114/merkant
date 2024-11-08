using System.Text.Json.Serialization;

namespace Common.EventBus.Integrations;

public record PurchaseCreatedIntegrationEvent : IntegrationEvent
{
    public long PurchaseId { get; private init; }
    public string UserId { get; private init; } = null!;
    public List<PurchaseItemCreatedIntegrationEvent> PurchaseItems { get; private init; }

    public PurchaseCreatedIntegrationEvent(long purchaseId, string userId, List<PurchaseItemCreatedIntegrationEvent> purchaseItems)
    {
        PurchaseId = purchaseId;
        UserId = userId;
        PurchaseItems = purchaseItems;
    }

    [JsonConstructor]
    public PurchaseCreatedIntegrationEvent(long purchaseId, string userId, List<PurchaseItemCreatedIntegrationEvent> purchaseItems, string id, DateTimeOffset creationDate, string? key) : base(id, creationDate, key)
    {
        PurchaseId = purchaseId;
        UserId = userId;
        PurchaseItems = purchaseItems;
    }
}