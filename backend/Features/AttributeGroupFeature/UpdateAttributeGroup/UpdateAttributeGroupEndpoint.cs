using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeGroupFeature.UpdateAttributeGroup;

[ApiController]
[Route("api/attribute-groups")]
[Tags("AttributeGroups")]
public class UpdateAttributeGroupController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateAttributeGroupResponse>>> UpdateAttributeGroup(
        long id,
        [FromBody] UpdateAttributeGroupRequest request)
    {
        var command = new UpdateAttributeGroupCommand(
            id,
            request.NameEn,
            request.NameAr,
            request.Slug,
            request.SortOrder,
            request.IsFilterable,
            request.Attributes);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
