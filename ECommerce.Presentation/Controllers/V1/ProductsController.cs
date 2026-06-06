using ECommerce.Application.Usecases.Products;
using ECommerce.Application.Usecases.Products.Create;
using ECommerce.Presentation.Contracts.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers.V1;


[Route("api/products")]
[Authorize]
public class ProductsController(ISender sender) : BaseController(sender)
{
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
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
    [Authorize(Policy = "AdminOnly")]
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
    public IActionResult GetById(Guid id)
    {
        return Ok(new { Id = id });
    }
}
