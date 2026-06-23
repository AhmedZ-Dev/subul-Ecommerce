using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.CreatePaymentMethod;

public class CreatePaymentMethodHandler(AppDbContext context)
    : IRequestHandler<CreatePaymentMethodCommand, Result<PaymentMethodResponse>>
{
    private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "offline", "online"
    };

    public async Task<Result<PaymentMethodResponse>> Handle(
        CreatePaymentMethodCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<PaymentMethodResponse>.Failure("Payment method name is required");

        var normalizedName = command.Name.Trim().ToLowerInvariant();

        var nameExists = await context.PaymentMethods.AnyAsync(
            pm => pm.Name.ToLower() == normalizedName,
            cancellationToken);

        if (nameExists)
            return Result<PaymentMethodResponse>.Failure("Payment method name already exists");

        if (!string.IsNullOrWhiteSpace(command.Type))
        {
            var typeNormalized = command.Type.Trim().ToLowerInvariant();
            if (!ValidTypes.Contains(typeNormalized))
                return Result<PaymentMethodResponse>.Failure("Invalid payment method type");

            command = command with { Type = typeNormalized };
        }

        var now = DateTime.Now;
        var paymentMethod = new PaymentMethod
        {
            Name = normalizedName,
            LabelEn = command.LabelEn?.Trim(),
            LabelAr = command.LabelAr?.Trim(),
            Type = command.Type?.Trim().ToLowerInvariant(),
            Gateway = command.Gateway?.Trim(),
            GatewayConfig = command.GatewayConfig,
            IconUrl = command.IconUrl,
            InstructionsEn = command.InstructionsEn,
            InstructionsAr = command.InstructionsAr,
            IsActive = command.IsActive,
            SortOrder = command.SortOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.PaymentMethods.Add(paymentMethod);
        await context.SaveChangesAsync(cancellationToken);

        return Result<PaymentMethodResponse>.Success(MapResponse(paymentMethod));
    }

    internal static PaymentMethodResponse MapResponse(PaymentMethod paymentMethod) =>
        new(
            paymentMethod.Id,
            paymentMethod.Name,
            paymentMethod.LabelEn,
            paymentMethod.LabelAr,
            paymentMethod.Type,
            paymentMethod.Gateway,
            paymentMethod.GatewayConfig,
            paymentMethod.IconUrl,
            paymentMethod.InstructionsEn,
            paymentMethod.InstructionsAr,
            paymentMethod.IsActive,
            paymentMethod.SortOrder,
            paymentMethod.CreatedAt,
            paymentMethod.UpdatedAt);
}
