using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.CreatePaymentMethod;

[ApiController]
[Route("api/payment-methods")]
[Tags("Payment Methods")]
public class CreatePaymentMethodController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentMethodResponse>>> CreatePaymentMethod(
        [FromBody] CreatePaymentMethodCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}
