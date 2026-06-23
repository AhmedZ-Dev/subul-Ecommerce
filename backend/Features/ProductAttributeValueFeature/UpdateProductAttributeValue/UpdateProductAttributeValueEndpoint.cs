using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductAttributeValueFeature.UpdateProductAttributeValue;

[ApiController]
[Route("api/products/{productId:long}/attribute-values/{id:long}")]
[Tags("Product Attribute Values")]
public class UpdateProductAttributeValueController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<ProductAttributeValueResponse>>> UpdateProductAttributeValue(
        long productId,
        long id,
        [FromBody] UpdateProductAttributeValueRequest request)
    {
        var command = new UpdateProductAttributeValueCommand(
            productId,
            id,
            request.AttributeId,
            request.ValueText,
            request.ValueNumber,
            request.ValueBoolean);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
