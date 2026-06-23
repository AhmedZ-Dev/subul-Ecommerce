using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductAttributeValueFeature.DeleteProductAttributeValue;

[ApiController]
[Route("api/products/{productId:long}/attribute-values/{id:long}")]
[Tags("Product Attribute Values")]
public class DeleteProductAttributeValueController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProductAttributeValue(long productId, long id)
    {
        var result = await sender.Send(new DeleteProductAttributeValueCommand(productId, id));
        return result.ToActionResult();
    }
}
