using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductImageFeature.CreateProductImage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductImageFeature.UpdateProductImage;

[ApiController]
[Route("api/products/{productId:long}/images/{id:long}")]
[Tags("Product Images")]
public class UpdateProductImageController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<ProductImageResponse>>> UpdateProductImage(
        long productId,
        long id,
        [FromBody] UpdateProductImageRequest request)
    {
        var command = new UpdateProductImageCommand(
            productId,
            id,
            request.VariantId,
            request.AltText,
            request.SortOrder,
            request.IsPrimary);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
