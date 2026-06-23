using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.UpdateBrand;

[ApiController]
[Route("api/brands")]
[Tags("Brands")]
public class UpdateBrandController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateBrandResponse>>> UpdateBrand(
        long id,
        [FromBody] UpdateBrandRequest request)
    {
        var command = new UpdateBrandCommand(
            id,
            request.Name,
            request.Slug,
            request.LogoUrl,
            request.BannerUrl,
            request.DescriptionEn,
            request.DescriptionAr,
            request.WebsiteUrl,
            request.IsFeatured,
            request.IsActive,
            request.SortOrder);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
