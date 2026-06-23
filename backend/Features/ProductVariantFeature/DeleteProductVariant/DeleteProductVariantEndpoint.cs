using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductVariantFeature.DeleteProductVariant;

[ApiController]
[Route("api/products/{productId:long}/variants/{id:long}")]
[Tags("Product Variants")]
public class DeleteProductVariantController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProductVariant(long productId, long id)
    {
        var result = await sender.Send(new DeleteProductVariantCommand(productId, id));
        return result.ToActionResult();
    }
}
