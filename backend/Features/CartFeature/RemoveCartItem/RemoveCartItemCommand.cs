using backend.Common.Results;
using MediatR;

namespace backend.Features.CartFeature.RemoveCartItem;

public record RemoveCartItemCommand(
    long CartItemId,
    string SessionId,
    long? UserId = null) : IRequest<Result<bool>>;
