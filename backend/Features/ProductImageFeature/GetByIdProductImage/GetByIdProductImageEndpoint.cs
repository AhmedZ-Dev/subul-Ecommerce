using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductImageFeature.CreateProductImage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductImageFeature.GetByIdProductImage;

[ApiController]
[Route("api/products/{productId:long}/images/{id:long}")]
[Tags("Product Images")]
public class GetByIdProductImageController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProductImageResponse>>> GetByIdProductImage(
        long productId,
        long id)
    {
        var result = await sender.Send(new GetByIdProductImageQuery(productId, id));
        return result.ToActionResult();
    }
}
