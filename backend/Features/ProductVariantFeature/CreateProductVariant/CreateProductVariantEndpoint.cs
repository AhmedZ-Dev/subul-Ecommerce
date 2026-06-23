using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductVariantFeature.CreateProductVariant;

[ApiController]
[Route("api/products/{productId:long}/variants")]
[Tags("Product Variants")]
public class CreateProductVariantController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductVariantResponse>>> CreateProductVariant(
        long productId,
        [FromBody] CreateProductVariantRequest request)
    {
        var command = new CreateProductVariantCommand(
            productId,
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
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
