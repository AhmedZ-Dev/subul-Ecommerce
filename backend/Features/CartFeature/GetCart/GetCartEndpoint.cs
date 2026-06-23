using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CartFeature.GetCart;

[ApiController]
[Route("api/carts")]
[Tags("Carts")]
public class GetCartController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartResponse>>> GetCart(
        [FromHeader(Name = "X-Cart-Session")] string? sessionId,
        [FromQuery] long? userId = null)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest(new ApiResponse<CartResponse>(false, default, "Cart session is required"));

        var result = await sender.Send(new GetCartQuery(sessionId, userId));
        return result.ToActionResult();
    }
}
