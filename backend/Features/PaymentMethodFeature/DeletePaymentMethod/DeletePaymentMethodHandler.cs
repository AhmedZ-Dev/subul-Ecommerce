using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.DeletePaymentMethod;

public class DeletePaymentMethodHandler(AppDbContext context)
    : IRequestHandler<DeletePaymentMethodCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeletePaymentMethodCommand command,
        CancellationToken cancellationToken)
    {
        var paymentMethod = await context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == command.Id, cancellationToken);

        if (paymentMethod is null)
            return Result<bool>.Failure("Payment method not found");

        var hasTransactions = await context.PaymentTransactions
            .AnyAsync(pt => pt.PaymentMethodId == command.Id, cancellationToken);

        if (hasTransactions)
            return Result<bool>.Failure(
                "Cannot delete payment method because it is associated with payment transactions");

        context.PaymentMethods.Remove(paymentMethod);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
