using ECommerce.Domain.Common.Events;
using ECommerce.Domain.Common.Events.Product;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Product.AddStock;

public class Product_ShouldRaiseStockAddedEvent
{
    [Fact]
    public void AddStock_Should_Raise_Event_When_Stock_Increases()
    {
        // Arrange
        var product = new ProductBuilder().WithStock(5).Build();
        product.ClearDomainEvents();

        // Act
        product.AddStock(3);

        // Assert
        product.Stock.Should().Be(8);
        product.DomainEvents.Should()
            .ContainSingle(e => e is ProductStockAddedEvent);
    }
}
