using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.ListCollectionPaginated;

[ApiController]
[Route("api/collections")]
[Tags("Collections")]
public class ListCollectionPaginatedController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListCollectionPaginatedResponse>>> ListCollectionPaginated(
        [FromQuery] ListCollectionPaginatedQuery query)
    {
        // Pin the visibility filter for anonymous callers so this public
        // route cannot be used to enumerate disabled collections.
        if (this.IsAnonymousCaller())
            query = query with { IsActive = true };

        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
