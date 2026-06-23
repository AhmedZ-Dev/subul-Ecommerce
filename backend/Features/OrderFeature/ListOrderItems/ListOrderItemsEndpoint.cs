using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.ListOrderItems;

[ApiController]
[Route("api/orders/{orderId:long}/items")]
[Tags("Orders")]
public class ListOrderItemsController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListOrderItemsResponse>>> ListOrderItems(long orderId)
    {
        var result = await sender.Send(new ListOrderItemsQuery(orderId));
        return result.ToActionResult();
    }
}
