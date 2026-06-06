namespace ECommerce.Domain.Common.Events.Product;

public class ProductStockAddedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public int Quantity { get; }

    public ProductStockAddedEvent(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}
