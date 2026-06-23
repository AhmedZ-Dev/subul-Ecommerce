using backend.Common.Results;
using MediatR;

namespace backend.Features.OrderFeature.GetByIdOrder;

public record GetByIdOrderQuery(long Id) : IRequest<Result<GetByIdOrderResponse>>;

public record OrderItemResponse(
    long Id,
    long OrderId,
    long? ProductId,
    long? VariantId,
    string ProductName,
    string? Sku,
    int Quantity,
    decimal UnitPrice,
    decimal? CompareAtPrice,
    decimal DiscountAmount,
    decimal TotalPrice,
    int WarrantyMonths,
    bool RequiresShipping,
    DateTime CreatedAt);

public record GetByIdOrderResponse(
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
    string? CouponCode,
    string? ShippingFirstName,
    string? ShippingLastName,
    string? ShippingPhone,
    string? ShippingAddress1,
    string? ShippingAddress2,
    string? ShippingCity,
    string? ShippingGovernorate,
    string ShippingCountry,
    long? ShippingZoneId,
    string? PaymentMethod,
    string? TrackingNumber,
    string? Notes,
    string? CustomerNotes,
    DateTime? CancelledAt,
    string? CancelReason,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<OrderItemResponse> Items);
