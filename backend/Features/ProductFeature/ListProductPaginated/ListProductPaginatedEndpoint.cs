using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.ListProductPaginated;

[ApiController]
[Route("api/products")]
[Tags("Products")]
public class ListProductPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListProductPaginatedResponse>>> ListProducts(
        [FromQuery] ListProductPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
