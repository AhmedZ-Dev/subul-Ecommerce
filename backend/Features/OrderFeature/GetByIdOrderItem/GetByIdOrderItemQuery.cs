using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using MediatR;

namespace backend.Features.OrderFeature.GetByIdOrderItem;

public record GetByIdOrderItemQuery(long OrderId, long ItemId) : IRequest<Result<OrderItemResponse>>;
