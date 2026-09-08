using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Common.Storage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.UploadBrandBanner;

[ApiController]
[Route("api/brands/{brandId:long}/banner")]
[Tags("Brands")]
public class UploadBrandBannerController(ISender sender) : ControllerBase
{
    [HttpPost]
    // Opts this action back up to the image size limit; the global cap is
    // sized for JSON. See ImageUploadSizeLimitAttribute.
    [ImageUploadSizeLimit]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<BrandImageAssetResponse>>> UploadBrandBanner(
        long brandId,
        [FromForm] UploadBrandBannerRequest request)
    {
        var command = new UploadBrandBannerCommand(brandId, request.Image);
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}

public class UploadBrandBannerRequest
{
    public IFormFile Image { get; set; } = null!;
}
