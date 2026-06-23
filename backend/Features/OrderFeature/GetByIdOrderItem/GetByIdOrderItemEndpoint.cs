using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.OrderFeature.GetByIdOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.GetByIdOrderItem;

[ApiController]
[Route("api/orders/{orderId:long}/items")]
[Tags("Orders")]
public class GetByIdOrderItemController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{itemId:long}")]
    public async Task<ActionResult<ApiResponse<OrderItemResponse>>> GetByIdOrderItem(
        long orderId,
        long itemId)
    {
        var result = await sender.Send(new GetByIdOrderItemQuery(orderId, itemId));
        return result.ToActionResult();
    }
}
