using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ShippingZoneFeature.DeleteShippingZone;

[ApiController]
[Route("api/shipping-zones/{id:long}")]
[Tags("ShippingZones")]
public class DeleteShippingZoneController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteShippingZone(long id)
    {
        var result = await sender.Send(new DeleteShippingZoneCommand(id));
        return result.ToActionResult();
    }
}
