using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.DeleteBrandBanner;

[ApiController]
[Route("api/brands/{brandId:long}/banner")]
[Tags("Brands")]
public class DeleteBrandBannerController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<BrandImageAssetResponse>>> DeleteBrandBanner(long brandId)
    {
        var result = await sender.Send(new DeleteBrandBannerCommand(brandId));
        return result.ToActionResult();
    }
}
