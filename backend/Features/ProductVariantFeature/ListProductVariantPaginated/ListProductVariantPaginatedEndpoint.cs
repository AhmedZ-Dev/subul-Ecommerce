using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductVariantFeature.ListProductVariantPaginated;

[ApiController]
[Route("api/products/{productId:long}/variants")]
[Tags("Product Variants")]
public class ListProductVariantPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListProductVariantPaginatedResponse>>> ListProductVariants(
        long productId,
        [FromQuery] ListProductVariantPaginatedQuery query)
    {
        var result = await sender.Send(query with { ProductId = productId });
        return result.ToActionResult();
    }
}
