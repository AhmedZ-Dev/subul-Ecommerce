using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeFeature.CreateAttribute;

[ApiController]
[Route("api/attribute-groups/{groupId:long}/attributes")]
[Tags("Attributes")]
public class CreateAttributeController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AttributeResponse>>> CreateAttribute(
        long groupId,
        [FromBody] CreateAttributeRequest request)
    {
        var command = new CreateAttributeCommand(
            groupId,
            request.NameEn,
            request.NameAr,
            request.Slug,
            request.Unit,
            request.InputType,
            request.IsFilterable,
            request.SortOrder);

        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
