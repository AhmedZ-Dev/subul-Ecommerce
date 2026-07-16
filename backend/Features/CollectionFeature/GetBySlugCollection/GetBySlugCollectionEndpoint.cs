using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.CollectionFeature.GetByIdCollection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.GetBySlugCollection;

[ApiController]
[Route("api/collections/by-slug")]
[Tags("Collections")]
public class GetBySlugCollectionController(ISender sender) : ControllerBase
{
    /// <summary>Storefront catalog — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet("{slug}")]
    public async Task<ActionResult<ApiResponse<GetByIdCollectionResponse>>> GetBySlugCollection(string slug)
    {
        var result = await sender.Send(new GetBySlugCollectionQuery(slug));
        return result.ToActionResult();
    }
}
