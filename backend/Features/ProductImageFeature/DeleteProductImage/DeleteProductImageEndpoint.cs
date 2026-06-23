using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductImageFeature.DeleteProductImage;

[ApiController]
[Route("api/products/{productId:long}/images/{id:long}")]
[Tags("Product Images")]
public class DeleteProductImageController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProductImage(long productId, long id)
    {
        var result = await sender.Send(new DeleteProductImageCommand(productId, id));
        return result.ToActionResult();
    }
}
