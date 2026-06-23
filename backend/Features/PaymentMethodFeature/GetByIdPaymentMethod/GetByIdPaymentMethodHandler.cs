using backend.Common.Results;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.GetByIdPaymentMethod;

public class GetByIdPaymentMethodHandler(AppDbContext context)
    : IRequestHandler<GetByIdPaymentMethodQuery, Result<PaymentMethodResponse>>
{
    public async Task<Result<PaymentMethodResponse>> Handle(
        GetByIdPaymentMethodQuery query,
        CancellationToken cancellationToken)
    {
        var paymentMethod = await context.PaymentMethods
            .AsNoTracking()
            .FirstOrDefaultAsync(pm => pm.Id == query.Id, cancellationToken);

        if (paymentMethod is null)
            return Result<PaymentMethodResponse>.Failure("Payment method not found");

        return Result<PaymentMethodResponse>.Success(
            CreatePaymentMethodHandler.MapResponse(paymentMethod));
    }
}
