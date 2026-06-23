using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.GetByIdCollection;

[ApiController]
[Route("api/collections")]
[Tags("Collections")]
public class GetByIdCollectionController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<GetByIdCollectionResponse>>> GetByIdCollection(long id)
    {
        var result = await sender.Send(new GetByIdCollectionQuery(id));
        return result.ToActionResult();
    }
}
