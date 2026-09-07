using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.ListCategoryPaginated;

[ApiController]
[Route("api/categories")]
[Tags("Categories")]
public class ListCategoryPaginatedController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListCategoryPaginatedResponse>>> ListCategories(
        [FromQuery] ListCategoryPaginatedQuery query)
    {
        // Pin the visibility filter for anonymous callers so this public
        // route cannot be used to enumerate disabled categories.
        if (this.IsAnonymousCaller())
            query = query with { IsActive = true };

        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
