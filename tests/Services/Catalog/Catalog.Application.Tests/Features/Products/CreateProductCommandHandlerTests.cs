using Catalog.Application.Common;
using ECommerce.BuildingBlocks.Application.Enums;
using Catalog.Application.Usecases.Products.Create;
using Catalog.Domain.Entities.Products;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Catalog.Application.Tests.Features.Products;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Product_And_SaveChanges()
    {
        // Arrange
        var uowMock = new Mock<IApplicationUnitOfWork>();
        var productsMock = new Mock<DbSet<Product>>();

        Product? capturedProduct = null;

        productsMock
            .Setup(x => x.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((product, _) =>
            {
                capturedProduct = product;
            });

        uowMock
            .Setup(x => x.Products)
            .Returns(productsMock.Object);

        var handler = new CreateProductCommandHandler(uowMock.Object);

        var command = new CreateProductCommand(
            "Laptop",
            1000m,
            5);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.None);
        result.Data.Should().NotBeEmpty();
        result.Message.Should().Be("Product created successfully.");

        capturedProduct.Should().NotBeNull();
        capturedProduct!.Id.Should().Be(result.Data);
        capturedProduct.Name.Should().Be("Laptop");
        capturedProduct.Price.Should().Be(1000m);
        capturedProduct.Stock.Should().Be(5);
    }
}