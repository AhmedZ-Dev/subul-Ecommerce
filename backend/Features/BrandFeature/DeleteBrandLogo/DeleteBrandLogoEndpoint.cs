using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.DeleteBrandLogo;

[ApiController]
[Route("api/brands/{brandId:long}/logo")]
[Tags("Brands")]
public class DeleteBrandLogoController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<BrandImageAssetResponse>>> DeleteBrandLogo(long brandId)
    {
        var result = await sender.Send(new DeleteBrandLogoCommand(brandId));
        return result.ToActionResult();
    }
}
