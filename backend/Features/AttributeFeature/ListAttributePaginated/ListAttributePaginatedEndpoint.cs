using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeFeature.ListAttributePaginated;

[ApiController]
[Route("api/attribute-groups/{groupId:long}/attributes")]
[Tags("Attributes")]
public class ListAttributePaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListAttributePaginatedResponse>>> ListAttributes(
        long groupId,
        [FromQuery] ListAttributePaginatedQuery query)
    {
        var result = await sender.Send(query with { GroupId = groupId });
        return result.ToActionResult();
    }
}
