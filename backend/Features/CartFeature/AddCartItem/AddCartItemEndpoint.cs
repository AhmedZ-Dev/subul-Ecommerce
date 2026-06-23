using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CartFeature.AddCartItem;

[ApiController]
[Route("api/carts/items")]
[Tags("Carts")]
public class AddCartItemController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AddCartItemResponse>>> AddCartItem(
        [FromHeader(Name = "X-Cart-Session")] string? sessionId,
        [FromBody] AddCartItemCommand command)
    {
        var merged = command with { SessionId = sessionId };
        var result = await sender.Send(merged);

        if (!result.IsSuccess)
            return result.ToActionResult();

        Response.Headers["X-Cart-Session"] = result.Value!.SessionId;
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
