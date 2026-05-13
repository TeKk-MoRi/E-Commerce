using MediatR;

namespace ECommerce.Application.Usecases.Products;

public record CreateProductCommand(string Name, decimal Price, int Stock) : IRequest<Guid>;

