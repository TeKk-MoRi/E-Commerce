namespace Catalog.Domain.Common.Events.Product;

public class ProductCreatedEvent : DomainEventBase
{
    public Guid ProductId { get; }

    public ProductCreatedEvent(Guid productId)
    {
        ProductId = productId;
    }
}
