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
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
