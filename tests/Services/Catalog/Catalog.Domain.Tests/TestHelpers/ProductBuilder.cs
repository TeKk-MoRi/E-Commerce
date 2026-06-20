using Catalog.Domain.Entities.Products;

namespace Catalog.Domain.Tests.TestHelpers;

public class ProductBuilder
{
    private string _name = "Test Product";
    private decimal _price = 100m;
    private int _stock = 10;

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithStock(int stock)
    {
        _stock = stock;
        return this;
    }

    public Catalog.Domain.Entities.Products.Product Build()
    {
        return Catalog.Domain.Entities.Products.Product.Create(_name, _price, _stock);
    }
}
