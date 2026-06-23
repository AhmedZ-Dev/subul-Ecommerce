using backend.Common.Results;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using MediatR;

namespace backend.Features.PaymentMethodFeature.GetByIdPaymentMethod;

public record GetByIdPaymentMethodQuery(long Id) : IRequest<Result<PaymentMethodResponse>>;
