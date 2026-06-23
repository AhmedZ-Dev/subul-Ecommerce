using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.AddressFeature.ListAddressPaginated;

[ApiController]
[Route("api/addresses")]
[Tags("Addresses")]
public class ListAddressPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListAddressPaginatedResponse>>> ListAddressPaginated(
        [FromQuery] ListAddressPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
