using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionProductFeature.ListCollectionProductPaginated;

[ApiController]
[Route("api/collections/{collectionId:long}/products")]
[Tags("Collection Products")]
public class ListCollectionProductPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListCollectionProductPaginatedResponse>>> ListCollectionProducts(
        long collectionId,
        [FromQuery] ListCollectionProductPaginatedQuery query)
    {
        var result = await sender.Send(query with { CollectionId = collectionId });
        return result.ToActionResult();
    }
}
