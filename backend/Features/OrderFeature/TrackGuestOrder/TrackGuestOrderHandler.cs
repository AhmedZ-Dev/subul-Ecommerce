using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.TrackGuestOrder;

public class TrackGuestOrderHandler(AppDbContext context)
    : IRequestHandler<TrackGuestOrderQuery, Result<TrackGuestOrderResponse>>
{
    public async Task<Result<TrackGuestOrderResponse>> Handle(
        TrackGuestOrderQuery query,
        CancellationToken cancellationToken)
    {
        var orderNumber = query.OrderNumber.Trim();
        var phone = NormalizePhone(query.Phone);

        if (string.IsNullOrWhiteSpace(orderNumber))
            return Result<TrackGuestOrderResponse>.Failure("Order number is required");

        if (string.IsNullOrWhiteSpace(phone))
            return Result<TrackGuestOrderResponse>.Failure("Phone is required");

        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

        if (order is null)
            return Result<TrackGuestOrderResponse>.Failure("Order not found");

        var orderPhone = NormalizePhone(order.ShippingPhone);
        if (!string.Equals(orderPhone, phone, StringComparison.Ordinal))
            return Result<TrackGuestOrderResponse>.Failure("Order not found");

        var items = order.OrderItems
            .Select(oi => new TrackGuestOrderItemResponse(
                oi.ProductName,
                oi.Quantity,
                oi.UnitPrice,
                oi.TotalPrice))
            .ToList();

        var response = new TrackGuestOrderResponse(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.PaymentStatus,
            order.FulfillmentStatus,
            order.Total,
            order.Currency,
            order.ShippingCity,
            order.ShippingGovernorate,
            order.TrackingNumber,
            order.ShippedAt,
            order.DeliveredAt,
            order.CreatedAt,
            items);

        return Result<TrackGuestOrderResponse>.Success(response);
    }

    private static string NormalizePhone(string? phone) =>
        string.IsNullOrWhiteSpace(phone)
            ? string.Empty
            : new string(phone.Where(char.IsDigit).ToArray());
}
