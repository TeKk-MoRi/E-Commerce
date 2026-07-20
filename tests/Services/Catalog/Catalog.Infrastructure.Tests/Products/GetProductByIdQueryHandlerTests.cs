using Catalog.Application.Usecases.Products.Get;
using Catalog.Domain.Entities.Products;
using Catalog.Infrastructure.Tests.TestHelpers;
using ECommerce.BuildingBlocks.Application.Enums;
using FluentAssertions;

namespace Catalog.Infrastructure.Tests.Products;

[Collection(CatalogDatabaseCollection.Name)]
public sealed class GetProductByIdQueryHandlerTests(CatalogDatabaseFixture databaseFixture) : IAsyncLifetime
{
    public Task InitializeAsync()
    {
        return databaseFixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnProduct()
    {
        await using var context =
            databaseFixture.CreateDbContext();

        var product = Product.Create(
            "Gaming Mouse",
            49.99m,
            10);

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler =
            new GetProductByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetProductByIdQuery(product.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data!.Id.Should().Be(product.Id);
        result.Data.Name.Should().Be("Gaming Mouse");
        result.Data.Price.Should().Be(49.99m);
        result.Data.Stock.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        await using var context =
            databaseFixture.CreateDbContext();

        var missingProductId = Guid.NewGuid();

        var handler =
            new GetProductByIdQueryHandler(context);

        var result = await handler.Handle(
            new GetProductByIdQuery(missingProductId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("Product.NotFound");
    }
}