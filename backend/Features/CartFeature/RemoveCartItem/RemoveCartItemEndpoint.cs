using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CartFeature.RemoveCartItem;

[ApiController]
[Route("api/carts/items")]
[Tags("Carts")]
public class RemoveCartItemController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveCartItem(
        long id,
        [FromHeader(Name = "X-Cart-Session")] string? sessionId,
        [FromQuery] long? userId = null)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest(new ApiResponse<bool>(false, default, "Cart session is required"));

        var result = await sender.Send(new RemoveCartItemCommand(id, sessionId, userId));
        return result.ToActionResult();
    }
}
