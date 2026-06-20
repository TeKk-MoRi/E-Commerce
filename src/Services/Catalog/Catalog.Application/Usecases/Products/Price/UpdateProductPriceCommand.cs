using Catalog.Application.Common;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Usecases.Products;

public sealed record UpdateProductPriceCommand(
    Guid ProductId,
    decimal NewPrice) : IRequest<Result>;

public class UpdateProductPriceCommandHandler(IApplicationUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductPriceCommand, Result>
{
    private readonly IApplicationUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(
        UpdateProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.NotFound(
                "Product.NotFound",
                "Product was not found.");
        }

        try
        {
            product.UpdatePrice(request.NewPrice);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Product price updated successfully.");
        }
        catch (DomainException ex)
        {
            return Result.ValidationFailure(
                "Product.Validation",
                ex.Message);
        }
    }
}