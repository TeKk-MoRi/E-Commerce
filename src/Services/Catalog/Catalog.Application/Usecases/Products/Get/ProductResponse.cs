namespace Catalog.Application.Usecases.Products.Get;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    int Stock);