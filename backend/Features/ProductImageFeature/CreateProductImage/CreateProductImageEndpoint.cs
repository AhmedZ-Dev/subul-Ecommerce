using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Common.Storage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ProductImageFeature.CreateProductImage;

[ApiController]
[Route("api/products/{productId:long}/images")]
[Tags("Product Images")]
public class CreateProductImageController(ISender sender) : ControllerBase
{
    [HttpPost]
    // Opts this action back up to the image size limit; the global cap is
    // sized for JSON. See ImageUploadSizeLimitAttribute.
    [ImageUploadSizeLimit]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<ProductImageResponse>>> CreateProductImage(
        long productId,
        [FromForm] CreateProductImageRequest request)
    {
        var command = new CreateProductImageCommand(
            productId,
            request.Image,
            request.VariantId,
            request.AltText,
            request.SortOrder,
            request.IsPrimary);

        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}

public class CreateProductImageRequest
{
    public IFormFile Image { get; set; } = null!;

    public long? VariantId { get; set; }

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}
