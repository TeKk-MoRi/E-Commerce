using Catalog.Domain.Common.Events;
using Catalog.Domain.Common.Events.Product;
using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.UpdatePrice;

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
