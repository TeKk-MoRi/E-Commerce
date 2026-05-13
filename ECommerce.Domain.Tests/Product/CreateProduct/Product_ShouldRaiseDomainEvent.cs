using ECommerce.Domain.Common.Events;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;

namespace ECommerce.Domain.Tests.Product.CreateProduct;

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
