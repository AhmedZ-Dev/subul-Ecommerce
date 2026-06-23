using backend.Common.Results;
using backend.Domain.Entities;
using backend.Features.OrderFeature.GetByIdOrder;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.OrderFeature.UpdateOrder;

public class UpdateOrderHandler(AppDbContext context)
    : IRequestHandler<UpdateOrderCommand, Result<GetByIdOrderResponse>>
{
    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "pending", "confirmed", "processing", "shipped", "out_for_delivery", "delivered", "cancelled", "refunded"
    };

    private static readonly HashSet<string> ValidPaymentStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "pending", "paid", "refunded"
    };

    private static readonly HashSet<string> ValidFulfillmentStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "unfulfilled", "partial", "fulfilled"
    };

    public async Task<Result<GetByIdOrderResponse>> Handle(
        UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == command.Id, cancellationToken);

        if (order is null)
            return Result<GetByIdOrderResponse>.Failure("Order not found");

        var now = DateTime.Now;
        var statusChanged = false;
        string? previousStatus = null;

        if (!string.IsNullOrWhiteSpace(command.Status))
        {
            var status = command.Status.Trim().ToLowerInvariant();
            if (!ValidStatuses.Contains(status))
                return Result<GetByIdOrderResponse>.Failure("Invalid order status");

            if (!string.Equals(order.Status, status, StringComparison.OrdinalIgnoreCase))
            {
                previousStatus = order.Status;
                order.Status = status;
                statusChanged = true;

                if (status is "cancelled" or "refunded")
                {
                    order.CancelledAt ??= now;
                    if (!string.IsNullOrWhiteSpace(command.CancelReason))
                        order.CancelReason = command.CancelReason.Trim();
                }

                if (status is "shipped" or "out_for_delivery")
                    order.ShippedAt ??= now;

                if (status == "delivered")
                {
                    order.ShippedAt ??= now;
                    order.DeliveredAt ??= now;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(command.PaymentStatus))
        {
            var paymentStatus = command.PaymentStatus.Trim().ToLowerInvariant();
            if (!ValidPaymentStatuses.Contains(paymentStatus))
                return Result<GetByIdOrderResponse>.Failure("Invalid payment status");

            order.PaymentStatus = paymentStatus;
        }

        if (!string.IsNullOrWhiteSpace(command.FulfillmentStatus))
        {
            var fulfillmentStatus = command.FulfillmentStatus.Trim().ToLowerInvariant();
            if (!ValidFulfillmentStatuses.Contains(fulfillmentStatus))
                return Result<GetByIdOrderResponse>.Failure("Invalid fulfillment status");

            order.FulfillmentStatus = fulfillmentStatus;
        }

        if (command.TrackingNumber is not null)
            order.TrackingNumber = string.IsNullOrWhiteSpace(command.TrackingNumber)
                ? null
                : command.TrackingNumber.Trim();

        if (command.Notes is not null)
            order.Notes = string.IsNullOrWhiteSpace(command.Notes) ? null : command.Notes.Trim();

        if (statusChanged)
        {
            order.OrderStatusHistories.Add(new OrderStatusHistory
            {
                FromStatus = previousStatus,
                ToStatus = order.Status,
                ChangedByType = "admin",
                Note = command.Notes?.Trim(),
                CreatedAt = now
            });
        }

        order.UpdatedAt = now;
        await context.SaveChangesAsync(cancellationToken);

        var response = GetByIdOrderHandler.MapOrder(order);
        return Result<GetByIdOrderResponse>.Success(response);
    }
}
