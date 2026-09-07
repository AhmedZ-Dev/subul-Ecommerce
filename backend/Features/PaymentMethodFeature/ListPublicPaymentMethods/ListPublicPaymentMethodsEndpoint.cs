using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.PaymentMethodFeature.ListPublicPaymentMethods;

[ApiController]
[Route("api/payment-methods/public")]
[Tags("Payment Methods")]
public class ListPublicPaymentMethodsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Storefront checkout — the only payment-method route that may stay
    /// [AllowAnonymous]. The admin list/get routes return GatewayConfig
    /// (gateway API keys) and must remain authenticated.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ListPublicPaymentMethodsResponse>>> ListPublicPaymentMethods()
    {
        var result = await sender.Send(new ListPublicPaymentMethodsQuery());
        return result.ToActionResult();
    }
}
