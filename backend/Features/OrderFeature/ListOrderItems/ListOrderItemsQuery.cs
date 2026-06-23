using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using MediatR;

namespace backend.Features.OrderFeature.ListOrderItems;

public record ListOrderItemsQuery(long OrderId) : IRequest<Result<ListOrderItemsResponse>>;

public record ListOrderItemsResponse(
    long OrderId,
    string OrderNumber,
    IReadOnlyList<OrderItemResponse> Items);
