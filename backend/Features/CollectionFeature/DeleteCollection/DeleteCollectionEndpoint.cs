using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.DeleteCollection;

[ApiController]
[Route("api/collections/{id:long}")]
[Tags("Collections")]
public class DeleteCollectionController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCollection(long id)
    {
        var result = await sender.Send(new DeleteCollectionCommand(id));
        return result.ToActionResult();
    }
}
