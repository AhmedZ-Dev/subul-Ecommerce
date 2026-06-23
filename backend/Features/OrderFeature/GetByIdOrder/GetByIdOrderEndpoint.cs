using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.GetByIdOrder;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
public class GetByIdOrderController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<GetByIdOrderResponse>>> GetByIdOrder(long id)
    {
        var result = await sender.Send(new GetByIdOrderQuery(id));
        return result.ToActionResult();
    }
}
