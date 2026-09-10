using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Common.Storage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.UploadBrandLogo;

[ApiController]
[Route("api/brands/{brandId:long}/logo")]
[Tags("Brands")]
public class UploadBrandLogoController(ISender sender) : ControllerBase
{
    [HttpPost]
    // Opts this action back up to the image size limit; the global cap is
    // sized for JSON. See ImageUploadSizeLimitAttribute.
    [ImageUploadSizeLimit]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<BrandImageAssetResponse>>> UploadBrandLogo(
        long brandId,
        [FromForm] UploadBrandLogoRequest request)
    {
        var command = new UploadBrandLogoCommand(brandId, request.Image);
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}

public class UploadBrandLogoRequest
{
    public IFormFile Image { get; set; } = null!;
}
