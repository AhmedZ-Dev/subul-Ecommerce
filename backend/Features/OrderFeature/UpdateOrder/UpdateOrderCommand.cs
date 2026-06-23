using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using MediatR;

namespace backend.Features.OrderFeature.UpdateOrder;

public record UpdateOrderCommand(
    long Id,
    string? Status = null,
    string? PaymentStatus = null,
    string? FulfillmentStatus = null,
    string? TrackingNumber = null,
    string? Notes = null,
    string? CancelReason = null) : IRequest<Result<GetByIdOrderResponse>>;
