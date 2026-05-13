using ECommerce.Domain.Common.Events;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Products.ProductTests.UpdatePrice;

public class Product_ShouldRaisePriceChangedEvent
{
    [Fact]
    public void UpdatePrice_Should_Raise_Event_When_Price_Changes()
    {
        // Arrange
        var product = new ProductBuilder().Build();

        // Act
        product.UpdatePrice(200m);

        // Assert
        product.DomainEvents.Should()
            .ContainSingle(e => e is ProductPriceChangedEvent);
    }
}
