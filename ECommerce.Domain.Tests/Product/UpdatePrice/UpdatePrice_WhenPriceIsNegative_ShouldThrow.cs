using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Products.ProductTests.UpdatePrice;

public class UpdatePrice_WhenPriceIsNegative_ShouldThrow
{
    [Fact]
    public void UpdatePrice_Should_Throw_DomainException_When_Price_Is_Negative()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        var act = () => product.UpdatePrice(-10m);

        // Act & Assert
        act.Should().Throw<DomainException>();
    }
}
