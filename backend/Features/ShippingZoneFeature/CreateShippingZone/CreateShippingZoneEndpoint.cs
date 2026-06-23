using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.ShippingZoneFeature.CreateShippingZone;

[ApiController]
[Route("api/shipping-zones")]
[Tags("ShippingZones")]
public class CreateShippingZoneController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateShippingZoneResponse>>> CreateShippingZone(
        [FromBody] CreateShippingZoneCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
