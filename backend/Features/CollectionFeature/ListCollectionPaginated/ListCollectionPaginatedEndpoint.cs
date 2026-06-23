using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.ListCollectionPaginated;

[ApiController]
[Route("api/collections")]
[Tags("Collections")]
public class ListCollectionPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListCollectionPaginatedResponse>>> ListCollectionPaginated(
        [FromQuery] ListCollectionPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
