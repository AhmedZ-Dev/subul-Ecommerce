using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AddressFeature.UpdateAddress;

[ApiController]
[Route("api/addresses")]
[Tags("Addresses")]
public class UpdateAddressController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UpdateAddressResponse>>> UpdateAddress(
        long id,
        [FromBody] UpdateAddressRequest request)
    {
        var command = new UpdateAddressCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Address1,
            request.Address2,
            request.City,
            request.Governorate,
            request.Country,
            request.IsDefault);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
