using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeGroupFeature.GetByIdAttributeGroup;

[ApiController]
[Route("api/attribute-groups")]
[Tags("AttributeGroups")]
public class GetByIdAttributeGroupController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<GetByIdAttributeGroupResponse>>> GetByIdAttributeGroup(long id)
    {
        var result = await sender.Send(new GetByIdAttributeGroupQuery(id));
        return result.ToActionResult();
    }
}
