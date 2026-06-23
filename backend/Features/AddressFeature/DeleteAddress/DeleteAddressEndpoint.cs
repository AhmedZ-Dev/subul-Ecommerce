using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AddressFeature.DeleteAddress;

[ApiController]
[Route("api/addresses/{id:long}")]
[Tags("Addresses")]
public class DeleteAddressController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAddress(long id)
    {
        var result = await sender.Send(new DeleteAddressCommand(id));
        return result.ToActionResult();
    }
}
