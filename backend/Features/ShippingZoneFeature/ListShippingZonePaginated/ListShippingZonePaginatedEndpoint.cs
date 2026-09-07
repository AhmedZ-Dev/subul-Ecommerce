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
        // Pin the visibility filter for anonymous callers so this public
        // route cannot be used to enumerate disabled shipping zones.
        if (this.IsAnonymousCaller())
            query = query with { IsActive = true };

        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
