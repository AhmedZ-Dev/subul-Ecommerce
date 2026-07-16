using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.ChangePaymentMethodStatus;

[ApiController]
[Route("api/payment-methods")]
[Tags("PaymentMethods")]
public class ChangePaymentMethodStatusController(ISender sender) : ControllerBase
{
    [HttpPut("{id:long}/status")]
    public async Task<ActionResult<ApiResponse<ChangePaymentMethodStatusResponse>>> ChangePaymentMethodStatus(
        long id,
        [FromBody] ChangePaymentMethodStatusRequest request)
    {
        var command = new ChangePaymentMethodStatusCommand(id, request.IsActive);
        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
