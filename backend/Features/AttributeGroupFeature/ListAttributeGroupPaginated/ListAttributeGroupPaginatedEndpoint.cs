using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeGroupFeature.ListAttributeGroupPaginated;

[ApiController]
[Route("api/attribute-groups")]
[Tags("AttributeGroups")]
public class ListAttributeGroupPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListAttributeGroupPaginatedResponse>>> ListAttributeGroupPaginated(
        [FromQuery] ListAttributeGroupPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
