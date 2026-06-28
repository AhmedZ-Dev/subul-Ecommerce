using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CategoryFeature.ChangeCategoryStatus;

[ApiController]
[Route("api/categories")]
[Tags("Categories")]
public class ChangeCategoryStatusController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}/status")]
    public async Task<ActionResult<ApiResponse<ChangeCategoryStatusResponse>>> ChangeCategoryStatus(
        long id,
        [FromBody] ChangeCategoryStatusRequest request)
    {
        var command = new ChangeCategoryStatusCommand(id, request.IsActive);
        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
