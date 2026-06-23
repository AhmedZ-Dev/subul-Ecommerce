using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeGroupFeature.CreateAttributeGroup;

[ApiController]
[Route("api/attribute-groups")]
[Tags("AttributeGroups")]
public class CreateAttributeGroupController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateAttributeGroupResponse>>> CreateAttributeGroup(
        [FromBody] CreateAttributeGroupCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
