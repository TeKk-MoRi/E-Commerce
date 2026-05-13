using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Products.ProductTests.AddStock;

public class AddStock_WhenQuantityIsInvalid_ShouldThrow
{
    [Fact]
    public void AddStock_Should_Throw_DomainException_When_Quantity_Is_Invalid()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        var act = () => product.AddStock(0);

        // Act & Assert
        act.Should().Throw<DomainException>();
    }
}
