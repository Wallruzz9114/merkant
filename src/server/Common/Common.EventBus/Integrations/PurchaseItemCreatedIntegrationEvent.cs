namespace Common.EventBus.Integrations;

public record PurchaseItemCreatedIntegrationEvent
{
    public string ProductId { get; private init; } = null!;
    public int Quantity { get; private init; }
    public decimal PaidPrice { get; private init; }
    public decimal? SuggestedPrice { get; private init; }
    public bool IsSuggestedPrice { get; private init; }

    public PurchaseItemCreatedIntegrationEvent(string productId, int quantity, decimal paidPrice, decimal? suggestedPrice, bool isSuggestedPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        PaidPrice = paidPrice;
        SuggestedPrice = suggestedPrice;
        IsSuggestedPrice = isSuggestedPrice;
    }
}