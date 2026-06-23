using backend.Common.Results;
using MediatR;

namespace backend.Features.OrderFeature.TrackGuestOrder;

public record TrackGuestOrderQuery(string OrderNumber, string Phone) : IRequest<Result<TrackGuestOrderResponse>>;

public record TrackGuestOrderResponse(
    long Id,
    string OrderNumber,
    string Status,
    string PaymentStatus,
    string FulfillmentStatus,
    decimal Total,
    string Currency,
    string? ShippingCity,
    string? ShippingGovernorate,
    string? TrackingNumber,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime CreatedAt,
    IReadOnlyList<TrackGuestOrderItemResponse> Items);

public record TrackGuestOrderItemResponse(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
