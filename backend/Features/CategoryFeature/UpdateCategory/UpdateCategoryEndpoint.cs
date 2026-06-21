using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.UpdateCategory;

[ApiController]
[Route("api/categories")]
[Tags("Categories")]
public class UpdateCategoryController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateCategoryResponse>>> UpdateCategory(
        long id,
        [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.NameEn,
            request.NameAr,
            request.DescriptionEn,
            request.DescriptionAr,
            request.ParentId,
            request.Slug,
            request.ImageUrl,
            request.BannerUrl,
            request.SortOrder,
            request.IsActive,
            request.SeoTitle,
            request.SeoDescription);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
