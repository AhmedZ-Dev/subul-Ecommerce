using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.DeletePaymentMethod;

[ApiController]
[Route("api/payment-methods/{id:long}")]
[Tags("Payment Methods")]
public class DeletePaymentMethodController(ISender sender) : ControllerBase
{
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePaymentMethod(long id)
    {
        var result = await sender.Send(new DeletePaymentMethodCommand(id));
        return result.ToActionResult();
    }
}
