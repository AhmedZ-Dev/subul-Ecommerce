using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.GetByIdOrderItem;

public class GetByIdOrderItemHandler(AppDbContext context)
    : IRequestHandler<GetByIdOrderItemQuery, Result<OrderItemResponse>>
{
    public async Task<Result<OrderItemResponse>> Handle(
        GetByIdOrderItemQuery query,
        CancellationToken cancellationToken)
    {
        var orderItem = await context.OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(
                oi => oi.Id == query.ItemId && oi.OrderId == query.OrderId,
                cancellationToken);

        if (orderItem is null)
            return Result<OrderItemResponse>.Failure("Order item not found");

        return Result<OrderItemResponse>.Success(GetByIdOrderHandler.MapOrderItem(orderItem));
    }
}
