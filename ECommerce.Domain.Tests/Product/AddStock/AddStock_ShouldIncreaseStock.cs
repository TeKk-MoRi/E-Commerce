using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Product.AddStock;

public class AddStock_ShouldIncreaseStock
{
    [Fact]
    public void AddStock_Should_Increase_Stock_When_Quantity_Is_Valid()
    {
        // Arrange
        var product = new ProductBuilder().WithStock(5).Build();

        // Act
        product.AddStock(3);

        // Assert
        product.Stock.Should().Be(8);
    }
}
