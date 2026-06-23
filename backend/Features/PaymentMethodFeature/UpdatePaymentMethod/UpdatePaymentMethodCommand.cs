using backend.Common.Results;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using MediatR;

namespace backend.Features.PaymentMethodFeature.UpdatePaymentMethod;

public record UpdatePaymentMethodCommand(
    long Id,
    string Name,
    string? LabelEn,
    string? LabelAr,
    string? Type,
    string? Gateway,
    string? GatewayConfig,
    string? IconUrl,
    string? InstructionsEn,
    string? InstructionsAr,
    bool IsActive,
    int SortOrder) : IRequest<Result<PaymentMethodResponse>>;

public record UpdatePaymentMethodRequest(
    string Name,
    string? LabelEn,
    string? LabelAr,
    string? Type,
    string? Gateway,
    string? GatewayConfig,
    string? IconUrl,
    string? InstructionsEn,
    string? InstructionsAr,
    bool IsActive,
    int SortOrder);
