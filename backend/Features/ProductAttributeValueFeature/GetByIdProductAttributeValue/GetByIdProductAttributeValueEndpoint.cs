using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductAttributeValueFeature.GetByIdProductAttributeValue;

[ApiController]
[Route("api/products/{productId:long}/attribute-values/{id:long}")]
[Tags("Product Attribute Values")]
public class GetByIdProductAttributeValueController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProductAttributeValueResponse>>> GetByIdProductAttributeValue(
        long productId,
        long id)
    {
        var result = await sender.Send(new GetByIdProductAttributeValueQuery(productId, id));
        return result.ToActionResult();
    }
}
