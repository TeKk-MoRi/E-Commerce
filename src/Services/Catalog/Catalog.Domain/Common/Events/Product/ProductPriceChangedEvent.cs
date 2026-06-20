namespace Catalog.Domain.Common.Events.Product;

public class ProductPriceChangedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public decimal NewPrice { get; }

    public ProductPriceChangedEvent(Guid productId, decimal newPrice)
    {
        ProductId = productId;
        NewPrice = newPrice;
    }
}
