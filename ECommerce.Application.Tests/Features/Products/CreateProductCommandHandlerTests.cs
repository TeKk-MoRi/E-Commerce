using ECommerce.Application.Common;
using ECommerce.Application.Usecases.Products;
using ECommerce.Domain.Entities.Products;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerce.Application.Tests.Features.Products;


public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Product_And_SaveChanges()
    {
        // Arrange
        var uowMock = new Mock<IApplicationUnitOfWork>();

        var productsMock = new Mock<DbSet<Product>>();

        uowMock.Setup(x => x.Products)
               .Returns(productsMock.Object);

        var handler = new CreateProductCommandHandler(uowMock.Object);

        var command = new CreateProductCommand(
            "Laptop",
            1000m,
            5);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        productsMock.Verify(x =>
            x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);

        uowMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        result.Should().NotBeEmpty();
    }
}