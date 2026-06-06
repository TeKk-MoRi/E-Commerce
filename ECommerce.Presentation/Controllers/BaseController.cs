using ECommerce.Application.Common;
using ECommerce.Application.Common.Enums;
using ECommerce.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers;

[ApiController]
public abstract class BaseController(ISender sender) : ControllerBase
{
    protected readonly ISender Sender = sender;

    protected IActionResult OkResult<T>(Result<T> result)
    {
        if (result.Error.Type == ErrorType.None)
            return Ok(result);

        return ProblemResult(result);
    }

    protected IActionResult OkResult(Result result)
    {
        if (result.Error.Type == ErrorType.None)
            return Ok(result);

        return ProblemResult(result);
    }

    protected IActionResult NoContentResult(Result result)
    {
        if (result.Error.Type == ErrorType.None)
            return NoContent();

        return ProblemResult(result);
    }

    protected IActionResult CreatedResult<T>(
        Result<T> result,
        string actionName,
        object routeValues)
    {
        if (result.Error.Type != ErrorType.None)
            return ProblemResult(result);

        return CreatedAtAction(
            actionName,
            routeValues,
            result);
    }

    private IActionResult ProblemResult(Result result)
    {
        var problemDetails = result.MapToProblemDetails();

        return Problem(
            detail: problemDetails.Detail,
            instance: problemDetails.Instance,
            statusCode: problemDetails.Status,
            title: problemDetails.Title,
            type: problemDetails.Type);
    }
}