using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.ListPaymentMethodPaginated;

[ApiController]
[Route("api/payment-methods")]
[Tags("Payment Methods")]
public class ListPaymentMethodPaginatedController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListPaymentMethodPaginatedResponse>>> ListPaymentMethods(
        [FromQuery] ListPaymentMethodPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
