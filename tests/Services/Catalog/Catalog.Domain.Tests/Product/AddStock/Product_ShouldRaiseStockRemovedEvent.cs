using Catalog.Domain.Common.Events;
using Catalog.Domain.Common.Events.Product;
using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.AddStock;

public class Product_ShouldRaiseStockRemovedEvent
{
    [Fact]
    public void RemoveStock_Should_Raise_Event_When_Stock_Decreases()
    {
        // Arrange
        var product = new ProductBuilder().WithStock(10).Build();
        product.ClearDomainEvents();

        // Act
        product.RemoveStock(4);

        // Assert
        product.Stock.Should().Be(6);
        product.DomainEvents.Should()
            .ContainSingle(e => e is ProductStockRemovedEvent);
    }
}
