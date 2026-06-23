using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ShippingZoneFeature.UpdateShippingZone;

[ApiController]
[Route("api/shipping-zones")]
[Tags("ShippingZones")]
public class UpdateShippingZoneController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateShippingZoneResponse>>> UpdateShippingZone(
        long id,
        [FromBody] UpdateShippingZoneRequest request)
    {
        var command = new UpdateShippingZoneCommand(
            id,
            request.NameEn,
            request.NameAr,
            request.Governorates,
            request.IsActive,
            request.ShippingRates);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
