using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.ListOrderItems;

public class ListOrderItemsHandler(AppDbContext context)
    : IRequestHandler<ListOrderItemsQuery, Result<ListOrderItemsResponse>>
{
    public async Task<Result<ListOrderItemsResponse>> Handle(
        ListOrderItemsQuery query,
        CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == query.OrderId, cancellationToken);

        if (order is null)
            return Result<ListOrderItemsResponse>.Failure("Order not found");

        var items = order.OrderItems
            .OrderBy(oi => oi.Id)
            .Select(GetByIdOrderHandler.MapOrderItem)
            .ToList();

        var response = new ListOrderItemsResponse(order.Id, order.OrderNumber, items);
        return Result<ListOrderItemsResponse>.Success(response);
    }
}
