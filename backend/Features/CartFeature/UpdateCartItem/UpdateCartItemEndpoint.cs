using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.CartFeature.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CartFeature.UpdateCartItem;

[ApiController]
[Route("api/carts/items")]
[Tags("Carts")]
public class UpdateCartItemController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateCartItem(
        long id,
        [FromHeader(Name = "X-Cart-Session")] string? sessionId,
        [FromBody] UpdateCartItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest(new ApiResponse<CartResponse>(false, default, "Cart session is required"));

        var command = new UpdateCartItemCommand(id, sessionId, request.Quantity, request.UserId);
        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}

public record UpdateCartItemRequest(int Quantity, long? UserId = null);
