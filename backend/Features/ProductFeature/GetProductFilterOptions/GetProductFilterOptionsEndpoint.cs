using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.GetProductFilterOptions;

[ApiController]
[Route("api/products")]
[Tags("Products")]
public class GetProductFilterOptionsController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog filters — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponse<ProductFilterOptionsResponse>>> GetFilterOptions(
        [FromQuery] long? categoryId)
    {
        var result = await sender.Send(new GetProductFilterOptionsQuery(categoryId));
        return result.ToActionResult();
    }
}
