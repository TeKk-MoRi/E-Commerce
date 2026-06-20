using Catalog.Domain.Common.Events;
using Catalog.Domain.Entities.Products;
using Catalog.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace Catalog.Domain.Tests.Product.CreateProduct;

public class Product_ShouldRaiseDomainEvent
{
    [Fact]
    public void Product_Should_Raise_ProductCreatedEvent_When_Created()
    {
        // Act
        var product = new ProductBuilder().Build();

        // Assert
        product.DomainEvents.Should().ContainSingle();
    }
}
