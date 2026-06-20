using Catalog.Domain.Entities.Products;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.CreateProduct;

public class Create_ShouldCreateProduct
{
    [Fact]
    public void Create_Should_Create_Product_When_Data_Is_Valid()
    {
        // Arrange
        var name = "Laptop";
        var price = 1000m;

        // Act
        var product = Catalog.Domain.Entities.Products.Product.Create(name, price);

        // Assert
        product.Name.Should().Be(name);
        product.Price.Should().Be(price);
        product.Stock.Should().Be(0);
    }
}
