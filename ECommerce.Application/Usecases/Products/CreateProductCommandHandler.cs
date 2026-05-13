using ECommerce.Application.Common;
using ECommerce.Domain.Entities.Products;
using MediatR;

namespace ECommerce.Application.Usecases.Products;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationUnitOfWork _uow;

    public CreateProductCommandHandler(IApplicationUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Price, request.Stock);
        await _uow.Products.AddAsync(product, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}