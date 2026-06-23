using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.TrackGuestOrder;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
public class TrackGuestOrderController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Guest order tracking — must remain [AllowAnonymous] when JWT auth is added.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("track")]
    public async Task<ActionResult<ApiResponse<TrackGuestOrderResponse>>> TrackGuestOrder(
        [FromQuery] string orderNumber,
        [FromQuery] string phone)
    {
        var result = await sender.Send(new TrackGuestOrderQuery(orderNumber, phone));
        return result.ToActionResult();
    }
}
