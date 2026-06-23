using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductVariantFeature.GetByIdProductVariant;

[ApiController]
[Route("api/products/{productId:long}/variants/{id:long}")]
[Tags("Product Variants")]
public class GetByIdProductVariantController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProductVariantResponse>>> GetByIdProductVariant(
        long productId,
        long id)
    {
        var result = await sender.Send(new GetByIdProductVariantQuery(productId, id));
        return result.ToActionResult();
    }
}
