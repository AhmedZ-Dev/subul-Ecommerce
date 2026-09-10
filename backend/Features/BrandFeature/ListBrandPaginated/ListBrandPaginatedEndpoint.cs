using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.BrandFeature.ListBrandPaginated;

[ApiController]
[Route("api/brands")]
[Tags("Brands")]
public class ListBrandPaginatedController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog filters — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListBrandPaginatedResponse>>> ListBrands(
        [FromQuery] ListBrandPaginatedQuery query)
    {
        // Pin the visibility filter for anonymous callers so this public
        // route cannot be used to enumerate disabled brands.
        if (this.IsAnonymousCaller())
            query = query with { IsActive = true };

        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
