using ECommerce.Domain.Common.Events;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Products.ProductTests.RemoveStock;

public class Product_ShouldRaiseStockRemovedEvent
{
    [Fact]
    public void RemoveStock_Should_Raise_Event_When_Stock_Decreases()
    {
        // Arrange
        var product = new ProductBuilder().WithStock(10).Build();

        // Act
        product.RemoveStock(4);

        // Assert
        product.Stock.Should().Be(6);
        product.DomainEvents.Should()
            .ContainSingle(e => e is ProductStockRemovedEvent);
    }
}
