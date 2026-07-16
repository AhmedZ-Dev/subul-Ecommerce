using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.ChangePaymentMethodStatus;

public class ChangePaymentMethodStatusHandler(AppDbContext context)
    : IRequestHandler<ChangePaymentMethodStatusCommand, Result<ChangePaymentMethodStatusResponse>>
{
    public async Task<Result<ChangePaymentMethodStatusResponse>> Handle(
        ChangePaymentMethodStatusCommand command,
        CancellationToken cancellationToken)
    {
        var paymentMethod = await context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == command.Id, cancellationToken);

        if (paymentMethod is null)
            return Result<ChangePaymentMethodStatusResponse>.Failure("Payment method not found");

        paymentMethod.IsActive = command.IsActive;
        paymentMethod.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        var response = new ChangePaymentMethodStatusResponse(
            paymentMethod.Id,
            paymentMethod.IsActive,
            paymentMethod.UpdatedAt);

        return Result<ChangePaymentMethodStatusResponse>.Success(response);
    }
}
