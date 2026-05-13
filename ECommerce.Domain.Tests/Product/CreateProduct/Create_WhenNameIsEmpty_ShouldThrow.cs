using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Products.ProductTests.Create;

public class Create_WhenNameIsEmpty_ShouldThrow
{
    [Fact]
    public void Create_Should_Throw_DomainException_When_Name_Is_Empty()
    {
        // Arrange
        var act = () => new ProductBuilder().WithName("").Build();

        // Act & Assert
        act.Should().Throw<DomainException>();
    }
}
