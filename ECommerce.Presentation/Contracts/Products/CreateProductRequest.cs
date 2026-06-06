namespace ECommerce.Presentation.Contracts.Products;

public sealed record CreateProductRequest(
    string Name,
    decimal Price,
    int Stock);