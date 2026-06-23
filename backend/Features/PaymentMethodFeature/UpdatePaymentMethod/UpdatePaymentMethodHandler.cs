using backend.Common.Results;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.UpdatePaymentMethod;

public class UpdatePaymentMethodHandler(AppDbContext context)
    : IRequestHandler<UpdatePaymentMethodCommand, Result<PaymentMethodResponse>>
{
    private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "offline", "online"
    };

    public async Task<Result<PaymentMethodResponse>> Handle(
        UpdatePaymentMethodCommand command,
        CancellationToken cancellationToken)
    {
        var paymentMethod = await context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == command.Id, cancellationToken);

        if (paymentMethod is null)
            return Result<PaymentMethodResponse>.Failure("Payment method not found");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<PaymentMethodResponse>.Failure("Payment method name is required");

        var normalizedName = command.Name.Trim().ToLowerInvariant();

        var nameExists = await context.PaymentMethods.AnyAsync(
            pm => pm.Name.ToLower() == normalizedName && pm.Id != command.Id,
            cancellationToken);

        if (nameExists)
            return Result<PaymentMethodResponse>.Failure("Payment method name already exists");

        string? normalizedType = null;
        if (!string.IsNullOrWhiteSpace(command.Type))
        {
            normalizedType = command.Type.Trim().ToLowerInvariant();
            if (!ValidTypes.Contains(normalizedType))
                return Result<PaymentMethodResponse>.Failure("Invalid payment method type");
        }

        paymentMethod.Name = normalizedName;
        paymentMethod.LabelEn = command.LabelEn?.Trim();
        paymentMethod.LabelAr = command.LabelAr?.Trim();
        paymentMethod.Type = normalizedType;
        paymentMethod.Gateway = command.Gateway?.Trim();
        paymentMethod.GatewayConfig = command.GatewayConfig;
        paymentMethod.IconUrl = command.IconUrl;
        paymentMethod.InstructionsEn = command.InstructionsEn;
        paymentMethod.InstructionsAr = command.InstructionsAr;
        paymentMethod.IsActive = command.IsActive;
        paymentMethod.SortOrder = command.SortOrder;
        paymentMethod.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        return Result<PaymentMethodResponse>.Success(
            CreatePaymentMethodHandler.MapResponse(paymentMethod));
    }
}
