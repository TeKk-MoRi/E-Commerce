using Catalog.Domain.Entities.Products;
using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.UpdatePrice;

public class UpdatePrice_ShouldUpdatePrice
{
    [Fact]
    public void UpdatePrice_Should_Update_Price_When_Value_Is_Valid()
    {
        // Arrange
        var product = new ProductBuilder().Build();

        // Act
        product.UpdatePrice(1200m);

        // Assert
        product.Price.Should().Be(1200m);
    }
}
