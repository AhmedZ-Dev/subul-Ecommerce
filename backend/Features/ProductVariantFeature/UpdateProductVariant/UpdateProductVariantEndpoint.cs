using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductVariantFeature.UpdateProductVariant;

[ApiController]
[Route("api/products/{productId:long}/variants/{id:long}")]
[Tags("Product Variants")]
public class UpdateProductVariantController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<ProductVariantResponse>>> UpdateProductVariant(
        long productId,
        long id,
        [FromBody] UpdateProductVariantRequest request)
    {
        var command = new UpdateProductVariantCommand(
            productId,
            id,
            request.Title,
            request.Sku,
            request.Barcode,
            request.Price,
            request.CompareAtPrice,
            request.CostPrice,
            request.StockQuantity,
            request.Weight,
            request.IsActive,
            request.SortOrder);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
