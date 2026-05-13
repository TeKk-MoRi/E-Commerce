namespace ECommerce.Domain.Common.Events;

public class ProductStockRemovedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public int Quantity { get; }

    public ProductStockRemovedEvent(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}
