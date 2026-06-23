using backend.Common.Results;
using backend.Features.CartFeature.GetCart;
using MediatR;

namespace backend.Features.CartFeature.MergeCart;

public record MergeCartCommand(string SessionId, long UserId) : IRequest<Result<CartResponse>>;
