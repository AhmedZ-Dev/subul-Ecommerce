using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ShippingZoneFeature.GetByIdShippingZone;

[ApiController]
[Route("api/shipping-zones")]
[Tags("ShippingZones")]
public class GetByIdShippingZoneController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<GetByIdShippingZoneResponse>>> GetByIdShippingZone(long id)
    {
        var result = await sender.Send(new GetByIdShippingZoneQuery(id));
        return result.ToActionResult();
    }
}
