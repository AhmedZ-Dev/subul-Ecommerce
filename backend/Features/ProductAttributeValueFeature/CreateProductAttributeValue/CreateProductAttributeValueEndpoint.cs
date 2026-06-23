using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;

[ApiController]
[Route("api/products/{productId:long}/attribute-values")]
[Tags("Product Attribute Values")]
public class CreateProductAttributeValueController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductAttributeValueResponse>>> CreateProductAttributeValue(
        long productId,
        [FromBody] CreateProductAttributeValueRequest request)
    {
        var command = new CreateProductAttributeValueCommand(
            productId,
            request.AttributeId,
            request.ValueText,
            request.ValueNumber,
            request.ValueBoolean);

        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
