using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.AttributeFeature.CreateAttribute;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AttributeFeature.GetByIdAttribute;

[ApiController]
[Route("api/attribute-groups/{groupId:long}/attributes/{id:long}")]
[Tags("Attributes")]
public class GetByIdAttributeController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<AttributeResponse>>> GetByIdAttribute(
        long groupId,
        long id)
    {
        var result = await sender.Send(new GetByIdAttributeQuery(groupId, id));
        return result.ToActionResult();
    }
}
