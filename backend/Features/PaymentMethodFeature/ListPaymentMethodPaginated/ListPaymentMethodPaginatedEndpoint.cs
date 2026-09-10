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
    /// <summary>
    /// Admin only — the response carries GatewayConfig (gateway API keys and
    /// webhook secrets). The storefront reads GET api/payment-methods/public,
    /// which projects a field set that is safe to publish.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListPaymentMethodPaginatedResponse>>> ListPaymentMethods(
        [FromQuery] ListPaymentMethodPaginatedQuery query)
    {
        var result = await sender.Send(query);
        return result.ToActionResult();
    }
}
