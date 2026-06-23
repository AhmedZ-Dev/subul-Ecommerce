using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductFeature.UpdateProduct;

[ApiController]
[Route("api/products")]
[Tags("Products")]
public class UpdateProductController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateProductResponse>>> UpdateProduct(
        long id,
        [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(
            id,
            request.NameEn,
            request.NameAr,
            request.CategoryId,
            request.BrandId,
            request.Slug,
            request.Sku,
            request.Barcode,
            request.DescriptionEn,
            request.DescriptionAr,
            request.ShortDescriptionEn,
            request.ShortDescriptionAr,
            request.Price,
            request.CompareAtPrice,
            request.CostPrice,
            request.Currency,
            request.StockQuantity,
            request.LowStockThreshold,
            request.MinOrderQuantity,
            request.Weight,
            request.Status,
            request.IsFeatured,
            request.RequiresShipping,
            request.WarrantyMonths,
            request.WarrantyDescription,
            request.MetaTitle,
            request.MetaDescription);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
