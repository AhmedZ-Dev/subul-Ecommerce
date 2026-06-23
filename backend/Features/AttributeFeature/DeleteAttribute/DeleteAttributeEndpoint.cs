using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeFeature.DeleteAttribute;

[ApiController]
[Route("api/attribute-groups/{groupId:long}/attributes/{id:long}")]
[Tags("Attributes")]
public class DeleteAttributeController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAttribute(long groupId, long id)
    {
        var result = await sender.Send(new DeleteAttributeCommand(groupId, id));
        return result.ToActionResult();
    }
}
