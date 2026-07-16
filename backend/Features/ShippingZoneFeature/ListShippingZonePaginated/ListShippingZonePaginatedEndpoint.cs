using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ShippingZoneFeature.ListShippingZonePaginated;

[ApiController]
[Route("api/shipping-zones")]
[Tags("ShippingZones")]
public class ListShippingZonePaginatedController(ISender sender) : ControllerBase
{
    /// <summary>Storefront checkout — must remain [AllowAnonymous].</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListShippingZonePaginatedResponse>>> ListShippingZonePaginated(
        [FromQuery] ListShippingZonePaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
