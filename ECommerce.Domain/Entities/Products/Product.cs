using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Events;
using ECommerce.Domain.Common.Events.Product;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.Entities.Products;

public class Product : Entity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { } // EF Core

    private Product(string name, decimal price, int stock)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
    }

    public static Product Create(string name, decimal price, int stock = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.");

        if (price < 0)
            throw new DomainException("Price cannot be negative.");

        if (stock < 0)
            throw new DomainException("Stock cannot be negative.");

        var product = new Product(name, price, stock);

        product.AddDomainEvent(new ProductCreatedEvent(product.Id));

        return product;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new DomainException("Price cannot be negative.");

        if (Price == newPrice)
            return;
        AddDomainEvent(new ProductPriceChangedEvent(Id, newPrice));

        Price = newPrice;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");

        Stock += quantity;

        AddDomainEvent(new ProductStockAddedEvent(Id, quantity));
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Stock quantity must be greater than zero");

        if (quantity > Stock)
            throw new DomainException("Insufficient stock");

        Stock -= quantity;
        AddDomainEvent(new ProductStockRemovedEvent(Id, quantity));
    }

}
