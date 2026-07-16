using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.CategoryFeature.GetByIdCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.GetBySlugCategory;

[ApiController]
[Route("api/categories/by-slug")]
[Tags("Categories")]
public class GetBySlugCategoryController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponse<GetByIdCategoryResponse>>> GetBySlugCategory(string slug)
    {
        var result = await sender.Send(new GetBySlugCategoryQuery(slug));
        return result.ToActionResult();
    }
}
