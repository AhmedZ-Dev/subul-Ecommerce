using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.CartFeature.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.CartFeature.MergeCart;

[ApiController]
[Route("api/carts")]
[Tags("Carts")]
public class MergeCartController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Merges a guest cart (session) into a registered user's cart after login.
    /// Keep [AllowAnonymous] until JWT is wired; callers supply userId explicitly for now.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("merge")]
    public async Task<ActionResult<ApiResponse<CartResponse>>> MergeCart(
        [FromHeader(Name = "X-Cart-Session")] string? sessionId,
        [FromBody] MergeCartRequest request)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest(new ApiResponse<CartResponse>(false, default, "Cart session is required"));

        var result = await sender.Send(new MergeCartCommand(sessionId, request.UserId));
        return result.ToActionResult();
    }
}

public record MergeCartRequest(long UserId);
