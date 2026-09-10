using backend.Common.Results;
using MediatR;

namespace backend.Features.PaymentMethodFeature.ListPublicPaymentMethods;

public record ListPublicPaymentMethodsQuery : IRequest<Result<ListPublicPaymentMethodsResponse>>;

public record ListPublicPaymentMethodsResponse(
    IReadOnlyList<PublicPaymentMethodResponse> Items);

/// <summary>
/// The storefront-safe projection of a payment method. It deliberately omits
/// <c>Gateway</c> and <c>GatewayConfig</c>: the latter holds the gateway's API
/// keys and webhook secrets, so it must never reach an unauthenticated caller.
/// Add new fields here only after confirming they are safe to publish.
/// </summary>
public record PublicPaymentMethodResponse(
    long Id,
    string Name,
    string? LabelEn,
    string? LabelAr,
    string? Type,
    string? IconUrl,
    string? InstructionsEn,
    string? InstructionsAr,
    int SortOrder);
