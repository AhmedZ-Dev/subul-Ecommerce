using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.GetByIdOrder;

public class GetByIdOrderHandler(AppDbContext context)
    : IRequestHandler<GetByIdOrderQuery, Result<GetByIdOrderResponse>>
{
    public async Task<Result<GetByIdOrderResponse>> Handle(
        GetByIdOrderQuery query,
        CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == query.Id, cancellationToken);

        if (order is null)
            return Result<GetByIdOrderResponse>.Failure("Order not found");

        var response = MapOrder(order);
        return Result<GetByIdOrderResponse>.Success(response);
    }

    internal static OrderItemResponse MapOrderItem(OrderItem item) =>
        new(
            item.Id,
            item.OrderId,
            item.ProductId,
            item.VariantId,
            item.ProductName,
            item.Sku,
            item.Quantity,
            item.UnitPrice,
            item.CompareAtPrice,
            item.DiscountAmount,
            item.TotalPrice,
            item.WarrantyMonths,
            item.RequiresShipping,
            item.CreatedAt);

    internal static GetByIdOrderResponse MapOrder(Order order)
    {
        var items = order.OrderItems
            .OrderBy(oi => oi.Id)
            .Select(MapOrderItem)
            .ToList();

        return new GetByIdOrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.Status,
            order.PaymentStatus,
            order.FulfillmentStatus,
            order.Subtotal,
            order.DiscountAmount,
            order.ShippingAmount,
            order.TaxAmount,
            order.Total,
            order.Currency,
            order.CouponCode,
            order.ShippingFirstName,
            order.ShippingLastName,
            order.ShippingPhone,
            order.ShippingAddress1,
            order.ShippingAddress2,
            order.ShippingCity,
            order.ShippingGovernorate,
            order.ShippingCountry,
            order.ShippingZoneId,
            order.PaymentMethod,
            order.TrackingNumber,
            order.Notes,
            order.CustomerNotes,
            order.CancelledAt,
            order.CancelReason,
            order.ShippedAt,
            order.DeliveredAt,
            order.CreatedAt,
            order.UpdatedAt,
            items);
    }
}
