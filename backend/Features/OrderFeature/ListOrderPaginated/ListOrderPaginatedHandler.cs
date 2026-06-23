using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.ListOrderPaginated;

public class ListOrderPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListOrderPaginatedQuery, Result<ListOrderPaginatedResponse>>
{
    public async Task<Result<ListOrderPaginatedResponse>> Handle(
        ListOrderPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var orderQuery = context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            orderQuery = orderQuery.Where(o =>
                o.OrderNumber.ToLower().Contains(search) ||
                (o.ShippingFirstName != null && o.ShippingFirstName.ToLower().Contains(search)) ||
                (o.ShippingLastName != null && o.ShippingLastName.ToLower().Contains(search)) ||
                (o.ShippingPhone != null && o.ShippingPhone.ToLower().Contains(search)) ||
                (o.TrackingNumber != null && o.TrackingNumber.ToLower().Contains(search)));
        }

        if (query.UserId is not null)
            orderQuery = orderQuery.Where(o => o.UserId == query.UserId);

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim().ToLower();
            orderQuery = orderQuery.Where(o => o.Status.ToLower() == status);
        }

        if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
        {
            var paymentStatus = query.PaymentStatus.Trim().ToLower();
            orderQuery = orderQuery.Where(o => o.PaymentStatus.ToLower() == paymentStatus);
        }

        if (!string.IsNullOrWhiteSpace(query.FulfillmentStatus))
        {
            var fulfillmentStatus = query.FulfillmentStatus.Trim().ToLower();
            orderQuery = orderQuery.Where(o => o.FulfillmentStatus.ToLower() == fulfillmentStatus);
        }

        orderQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("ordernumber", "asc") => orderQuery.OrderBy(o => o.OrderNumber),
            ("ordernumber", "desc") => orderQuery.OrderByDescending(o => o.OrderNumber),
            ("total", "asc") => orderQuery.OrderBy(o => o.Total),
            ("total", "desc") => orderQuery.OrderByDescending(o => o.Total),
            ("status", "asc") => orderQuery.OrderBy(o => o.Status),
            ("status", "desc") => orderQuery.OrderByDescending(o => o.Status),
            ("updatedat", "asc") => orderQuery.OrderBy(o => o.UpdatedAt),
            ("updatedat", "desc") => orderQuery.OrderByDescending(o => o.UpdatedAt),
            ("createdat", "asc") => orderQuery.OrderBy(o => o.CreatedAt),
            _ => orderQuery.OrderByDescending(o => o.CreatedAt)
        };

        var total = await orderQuery.CountAsync(cancellationToken);

        var orders = await orderQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = orders.Select(o =>
        {
            var orderItems = o.OrderItems
                .OrderBy(oi => oi.Id)
                .Select(GetByIdOrderHandler.MapOrderItem)
                .ToList();

            return new ListOrderPaginatedItemResponse(
                o.Id,
                o.OrderNumber,
                o.UserId,
                o.Status,
                o.PaymentStatus,
                o.FulfillmentStatus,
                o.Total,
                o.Currency,
                o.ShippingFirstName,
                o.ShippingPhone,
                o.ShippingCity,
                o.ShippingGovernorate,
                o.PaymentMethod,
                o.TrackingNumber,
                o.CreatedAt,
                o.UpdatedAt,
                orderItems.Count,
                orderItems);
        }).ToList();

        var response = new ListOrderPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListOrderPaginatedResponse>.Success(response);
    }
}
