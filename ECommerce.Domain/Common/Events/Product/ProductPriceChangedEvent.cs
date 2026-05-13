namespace ECommerce.Domain.Common.Events;

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
