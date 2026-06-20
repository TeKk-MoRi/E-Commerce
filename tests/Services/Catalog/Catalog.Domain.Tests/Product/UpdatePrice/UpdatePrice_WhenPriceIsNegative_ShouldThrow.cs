using Catalog.Domain.Entities.Products;
using Catalog.Domain.Exceptions;
using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.UpdatePrice;

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
