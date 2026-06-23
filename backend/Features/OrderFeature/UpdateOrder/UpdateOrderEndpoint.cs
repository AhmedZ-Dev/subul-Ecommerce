using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.OrderFeature.GetByIdOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.UpdateOrder;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
public class UpdateOrderController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<GetByIdOrderResponse>>> UpdateOrder(
        long id,
        [FromBody] UpdateOrderRequest request)
    {
        var command = new UpdateOrderCommand(
            id,
            request.Status,
            request.PaymentStatus,
            request.FulfillmentStatus,
            request.TrackingNumber,
            request.Notes,
            request.CancelReason);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}

public record UpdateOrderRequest(
    string? Status = null,
    string? PaymentStatus = null,
    string? FulfillmentStatus = null,
    string? TrackingNumber = null,
    string? Notes = null,
    string? CancelReason = null);
