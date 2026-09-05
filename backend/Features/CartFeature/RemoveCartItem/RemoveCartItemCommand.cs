using backend.Common.Results;
using MediatR;

namespace backend.Features.CartFeature.RemoveCartItem;

public record RemoveCartItemCommand(
    long CartItemId,
    string SessionId) : IRequest<Result<bool>>;
