using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AddressFeature.GetByIdAddress;

[ApiController]
[Route("api/addresses")]
[Tags("Addresses")]
public class GetByIdAddressController(ISender sender) : ControllerBase
{
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<GetByIdAddressResponse>>> GetByIdAddress(long id)
    {
        var result = await sender.Send(new GetByIdAddressQuery(id));
        return result.ToActionResult();
    }
}
