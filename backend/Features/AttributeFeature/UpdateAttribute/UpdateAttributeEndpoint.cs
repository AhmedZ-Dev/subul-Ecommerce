using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.AttributeFeature.CreateAttribute;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeFeature.UpdateAttribute;

[ApiController]
[Route("api/attribute-groups/{groupId:long}/attributes/{id:long}")]
[Tags("Attributes")]
public class UpdateAttributeController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<AttributeResponse>>> UpdateAttribute(
        long groupId,
        long id,
        [FromBody] UpdateAttributeRequest request)
    {
        var command = new UpdateAttributeCommand(
            groupId,
            id,
            request.NameEn,
            request.NameAr,
            request.Slug,
            request.Unit,
            request.InputType,
            request.IsFilterable,
            request.SortOrder);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
