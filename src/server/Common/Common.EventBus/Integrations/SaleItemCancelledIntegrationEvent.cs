namespace Common.EventBus.Integrations;

public record SaleItemCancelledIntegrationEvent
{
    public string ProductId { get; private init; } = null!;
    public int Quantity { get; private init; }
    public decimal Price { get; private init; }
}