using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeGroupFeature.DeleteAttributeGroup;

[ApiController]
[Route("api/attribute-groups/{id:long}")]
[Tags("AttributeGroups")]
public class DeleteAttributeGroupController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAttributeGroup(long id)
    {
        var result = await sender.Send(new DeleteAttributeGroupCommand(id));
        return result.ToActionResult();
    }
}
