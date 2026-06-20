using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.UpdatePrice;

public class UpdatePrice_WhenPriceIsSame_ShouldNotRaiseEvent
{
    [Fact]
    public void UpdatePrice_Should_Not_Raise_Event_When_Price_Is_Same()
    {
        // Arrange
        var product = new ProductBuilder()
            .WithPrice(100m)
            .Build();

        product.ClearDomainEvents();

        // Act
        product.UpdatePrice(100m);

        // Assert
        product.Price.Should().Be(100m);
        product.DomainEvents.Should().BeEmpty();
    }
}