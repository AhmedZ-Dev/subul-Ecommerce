using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CollectionFeature.CreateCollection;

[ApiController]
[Route("api/collections")]
[Tags("Collections")]
public class CreateCollectionController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateCollectionResponse>>> CreateCollection(
        [FromBody] CreateCollectionCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
