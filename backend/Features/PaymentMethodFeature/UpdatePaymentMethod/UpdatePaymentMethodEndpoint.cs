using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.UpdatePaymentMethod;

[ApiController]
[Route("api/payment-methods/{id:long}")]
[Tags("Payment Methods")]
public class UpdatePaymentMethodController(ISender sender) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<ApiResponse<PaymentMethodResponse>>> UpdatePaymentMethod(
        long id,
        [FromBody] UpdatePaymentMethodRequest request)
    {
        var command = new UpdatePaymentMethodCommand(
            id,
            request.Name,
            request.LabelEn,
            request.LabelAr,
            request.Type,
            request.Gateway,
            request.GatewayConfig,
            request.IconUrl,
            request.InstructionsEn,
            request.InstructionsAr,
            request.IsActive,
            request.SortOrder);

        var result = await sender.Send(command);
        return result.ToActionResult();
    }
}
