using Catalog.Domain.Entities.Products;
using Catalog.Infrastructure.Context;
using ECommerce.BuildingBlocks.Application.Enums;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Usecases.Products.Get;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;

namespace Catalog.Application.Tests.Features.Products
{
    public sealed class GetProductByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            var option = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
            await using var context = new ApplicationDbContext(option); 

            var product = Product.Create("Gaming Mouse", 49.99m, 10);

            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var handler = new GetProductByIdQueryHandler(context);

            var result = await handler.Handle(
                new GetProductByIdQuery(product.Id),
                CancellationToken.None);

            // Assert
            result.Data!.Id.Should().Be(product.Id);
            result.Data.Name.Should().Be("Gaming Mouse");
            result.Data.Price.Should().Be(49.99m);
            result.Data.Stock.Should().Be(10);
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_ShouldReturnNotFound()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            await using var context =
                new ApplicationDbContext(options);

            var handler =
                new GetProductByIdQueryHandler(context);

            var productId = Guid.NewGuid();

            var result = await handler.Handle(
                new GetProductByIdQuery(productId),
                CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Data.Should().BeNull();
            result.Error.Type.Should().Be(ErrorType.NotFound);
            result.Error.Code.Should().Be("Product.NotFound");
        }
    }
}
