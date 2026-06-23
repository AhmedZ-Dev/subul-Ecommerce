using backend.Common.Extensions;
using backend.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.OrderFeature.CreateOrder;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
public class CreateOrderController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Guest checkout endpoint — must remain [AllowAnonymous] when JWT auth is added.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateOrderResponse>>> CreateOrder(
        [FromHeader(Name = "X-Cart-Session")] string? sessionId,
        [FromBody] CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            return BadRequest(new ApiResponse<CreateOrderResponse>(false, default, "Cart session is required"));

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var command = new CreateOrderCommand(
            SessionId: sessionId,
            UserId: request.UserId,
            AddressId: request.AddressId,
            ShippingFirstName: request.ShippingFirstName,
            ShippingLastName: request.ShippingLastName,
            ShippingPhone: request.ShippingPhone,
            ShippingAddress1: request.ShippingAddress1,
            ShippingAddress2: request.ShippingAddress2,
            ShippingCity: request.ShippingCity,
            ShippingGovernorate: request.ShippingGovernorate,
            ShippingCountry: request.ShippingCountry,
            ShippingZoneId: request.ShippingZoneId,
            PaymentMethod: request.PaymentMethod,
            CustomerNotes: request.CustomerNotes,
            CouponCode: request.CouponCode,
            IpAddress: ipAddress);

        var result = await sender.Send(command);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}

public record CreateOrderRequest(
    long? UserId = null,
    long? AddressId = null,
    string? ShippingFirstName = null,
    string? ShippingLastName = null,
    string? ShippingPhone = null,
    string? ShippingAddress1 = null,
    string? ShippingAddress2 = null,
    string? ShippingCity = null,
    string? ShippingGovernorate = null,
    string? ShippingCountry = "Iraq",
    long? ShippingZoneId = null,
    string PaymentMethod = "cod",
    string? CustomerNotes = null,
    string? CouponCode = null);
