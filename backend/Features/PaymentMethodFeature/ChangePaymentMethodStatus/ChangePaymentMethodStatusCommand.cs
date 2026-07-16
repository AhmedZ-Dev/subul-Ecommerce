using backend.Common.Results;
using MediatR;

namespace backend.Features.PaymentMethodFeature.ChangePaymentMethodStatus;

public record ChangePaymentMethodStatusCommand(long Id, bool IsActive)
    : IRequest<Result<ChangePaymentMethodStatusResponse>>;

public record ChangePaymentMethodStatusRequest(bool IsActive);

public record ChangePaymentMethodStatusResponse(
    long Id,
    bool IsActive,
    DateTime? UpdatedAt);
