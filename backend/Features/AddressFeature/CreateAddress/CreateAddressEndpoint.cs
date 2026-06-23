using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AddressFeature.CreateAddress;

[ApiController]
[Route("api/addresses")]
[Tags("Addresses")]
public class CreateAddressController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateAddressResponse>>> CreateAddress(
        [FromBody] CreateAddressCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
