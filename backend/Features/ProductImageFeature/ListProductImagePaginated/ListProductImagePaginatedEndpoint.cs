using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductImageFeature.ListProductImagePaginated;

[ApiController]
[Route("api/products/{productId:long}/images")]
[Tags("Product Images")]
public class ListProductImagePaginatedController(ISender sender) : ControllerBase
{
    /// <summary>Storefront product gallery — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListProductImagePaginatedResponse>>> ListProductImages(
        long productId,
        [FromQuery] ListProductImagePaginatedQuery query)
    {
        var result = await sender.Send(query with { ProductId = productId });
        return result.ToActionResult();
    }
}
