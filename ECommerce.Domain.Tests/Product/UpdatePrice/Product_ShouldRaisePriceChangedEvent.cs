using ECommerce.Domain.Common.Events;
using ECommerce.Domain.Common.Events.Product;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Product.UpdatePrice;

public class Product_ShouldRaisePriceChangedEvent
{
    [Fact]
    public void UpdatePrice_Should_Raise_Event_When_Price_Changes()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        product.ClearDomainEvents();

        // Act
        product.UpdatePrice(200m);

        // Assert
        product.DomainEvents.Should()
            .ContainSingle(e => e is ProductPriceChangedEvent);
    }
}
