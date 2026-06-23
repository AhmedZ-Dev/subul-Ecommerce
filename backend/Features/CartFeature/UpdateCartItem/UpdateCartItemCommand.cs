using backend.Common.Results;
using backend.Features.CartFeature.GetCart;
using MediatR;

namespace backend.Features.CartFeature.UpdateCartItem;

public record UpdateCartItemCommand(
    long CartItemId,
    string SessionId,
    int Quantity,
    long? UserId = null) : IRequest<Result<CartResponse>>;
