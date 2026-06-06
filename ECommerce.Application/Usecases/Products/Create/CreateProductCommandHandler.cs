using ECommerce.Application.Common;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Exceptions;
using MediatR;

namespace ECommerce.Application.Usecases.Products.Create;

public record CreateProductCommand(
    string Name,
    decimal Price,
    int Stock) : IRequest<Result<Guid>>;

public class CreateProductCommandHandler(IApplicationUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = Product.Create(
                request.Name,
                request.Price,
                request.Stock);

            await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(
                product.Id,
                "Product created successfully.");
        }
        catch (DomainException ex)
        {
            return Result<Guid>.ValidationFailure(
                "Product.Validation",
                ex.Message);
        }
    }
}