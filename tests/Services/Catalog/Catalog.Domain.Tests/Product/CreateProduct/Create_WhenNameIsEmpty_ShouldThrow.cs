using Catalog.Domain.Entities.Products;
using Catalog.Domain.Exceptions;
using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.CreateProduct;

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
