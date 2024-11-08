using System.Text.Json.Serialization;

namespace Common.EventBus.Integrations;

public record SaleCancelledIntegrationEvent : IntegrationEvent
{
    public long SaleId { get; private init; }
    public string? UserId { get; private init; }
    public List<SaleItemCancelledIntegrationEvent>? SaleItems { get; private init; }

    public SaleCancelledIntegrationEvent(long saleId, string userId, List<SaleItemCancelledIntegrationEvent> saleItems)
    {
        SaleId = saleId;
        UserId = userId;
        SaleItems = saleItems;
    }

    [JsonConstructor]
    public SaleCancelledIntegrationEvent(long saleId, string userId, List<SaleItemCancelledIntegrationEvent> saleItems, string id, DateTimeOffset creationDate, string? key) : base(id, creationDate, key)
    {
        SaleId = saleId;
        UserId = userId;
        SaleItems = saleItems;
    }
}