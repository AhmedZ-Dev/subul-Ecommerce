using backend.Common.Results;
using MediatR;

namespace backend.Features.OrderFeature.CreateOrder;

public record CreateOrderCommand(
    string SessionId,
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
    string? CouponCode = null,
    string? IpAddress = null) : IRequest<Result<CreateOrderResponse>>;

public record CreateOrderResponse(
    long Id,
    string OrderNumber,
    long? UserId,
    string Status,
    string PaymentStatus,
    string FulfillmentStatus,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal ShippingAmount,
    decimal TaxAmount,
    decimal Total,
    string Currency,
    string? ShippingFirstName,
    string? ShippingLastName,
    string? ShippingPhone,
    string? ShippingAddress1,
    string? ShippingCity,
    string? ShippingGovernorate,
    string ShippingCountry,
    long? ShippingZoneId,
    string? PaymentMethod,
    DateTime CreatedAt,
    IReadOnlyList<CreateOrderItemResponse> Items);

public record CreateOrderItemResponse(
    long Id,
    long? ProductId,
    long? VariantId,
    string ProductName,
    string? Sku,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
