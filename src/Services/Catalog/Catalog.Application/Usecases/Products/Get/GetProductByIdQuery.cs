using Catalog.Application.Common;
using ECommerce.BuildingBlocks.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Usecases.Products.Get;

public sealed record GetProductByIdQuery(Guid Id)
    : IRequest<Result<ProductResponse>>;

public sealed class GetProductByIdQueryHandler(
    IApplicationUnitOfWork unitOfWork)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new ProductResponse(
                x.Id,
                x.Name,
                x.Price,
                x.Stock))
            .SingleOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return Result<ProductResponse>.NotFound(
                "Product.NotFound",
                $"Product with ID '{request.Id}' was not found.");
        }

        return Result<ProductResponse>.Success(product);
    }
}