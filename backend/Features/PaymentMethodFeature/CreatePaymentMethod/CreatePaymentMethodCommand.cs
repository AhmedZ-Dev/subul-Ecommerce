using backend.Common.Results;
using MediatR;

namespace backend.Features.PaymentMethodFeature.CreatePaymentMethod;

public record CreatePaymentMethodCommand(
    string Name,
    string? LabelEn = null,
    string? LabelAr = null,
    string? Type = null,
    string? Gateway = null,
    string? GatewayConfig = null,
    string? IconUrl = null,
    string? InstructionsEn = null,
    string? InstructionsAr = null,
    bool IsActive = true,
    int SortOrder = 0) : IRequest<Result<PaymentMethodResponse>>;

public record PaymentMethodResponse(
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
    int SortOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
