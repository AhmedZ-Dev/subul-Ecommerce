using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductAttributeValueFeature.ListProductAttributeValuePaginated;

[ApiController]
[Route("api/products/{productId:long}/attribute-values")]
[Tags("Product Attribute Values")]
public class ListProductAttributeValuePaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListProductAttributeValuePaginatedResponse>>> ListProductAttributeValues(
        long productId,
        [FromQuery] ListProductAttributeValuePaginatedQuery query)
    {
        var result = await sender.Send(query with { ProductId = productId });
        return result.ToActionResult();
    }
}
