using Catalog.Application.Usecases.Products;
using Catalog.Application.Usecases.Products.Price;
using Catalog.Domain.Entities.Products;
using Catalog.Infrastructure.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Tests.Features.Products;

public class UpdateProductPriceCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Product_Price_When_Product_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var product = Product.Create("Gaming Mouse", 49.99m, 10);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new UpdateProductPriceCommandHandler(context);

        var command = new UpdateProductPriceCommand(product.Id, 59.99m);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await context.Products
            .FirstAsync(x => x.Id == product.Id);

        updatedProduct.Price.Should().Be(59.99m);
    }
}