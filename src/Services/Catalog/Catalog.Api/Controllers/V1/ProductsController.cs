using Catalog.Api.Contracts.Products;
using Catalog.Application.Usecases.Products;
using Catalog.Application.Usecases.Products.Create;
using Catalog.Application.Usecases.Products.Get;
using Catalog.Contracts.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers.V1;

[Route("api/products")]
[Authorize]
public class ProductsController(ISender sender) : BaseController(sender)
{
    [HttpPost]
    [Authorize(Policy = CatalogPermissions.ProductsManage)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new CreateProductCommand(
                request.Name,
                request.Price,
                request.Stock),
            cancellationToken);

        return CreatedResult(
            result,
            nameof(GetById),
            new { id = result.Data });
    }

    [HttpPut("{id:guid}/price")]
    [Authorize(Policy = CatalogPermissions.ProductsManage)]
    public async Task<IActionResult> UpdatePrice(
        Guid id,
        [FromBody] UpdateProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateProductPriceCommand(id, request.NewPrice),
            cancellationToken);

        return NoContentResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = CatalogPermissions.ProductsView)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return OkResult(result);
    }
}