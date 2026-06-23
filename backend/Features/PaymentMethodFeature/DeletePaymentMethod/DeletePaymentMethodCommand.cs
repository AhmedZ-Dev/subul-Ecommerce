using backend.Common.Results;
using MediatR;

namespace backend.Features.PaymentMethodFeature.DeletePaymentMethod;

public record DeletePaymentMethodCommand(long Id) : IRequest<Result<bool>>;
