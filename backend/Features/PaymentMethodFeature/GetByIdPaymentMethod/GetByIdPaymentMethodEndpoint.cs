using backend.Common.Extensions;
using backend.Common.Responses;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.GetByIdPaymentMethod;

[ApiController]
[Route("api/payment-methods/{id:long}")]
[Tags("Payment Methods")]
public class GetByIdPaymentMethodController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaymentMethodResponse>>> GetByIdPaymentMethod(long id)
    {
        var result = await sender.Send(new GetByIdPaymentMethodQuery(id));
        return result.ToActionResult();
    }
}
