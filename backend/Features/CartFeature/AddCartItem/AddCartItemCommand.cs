using backend.Common.Results;
using backend.Features.CartFeature.GetCart;
using MediatR;

namespace backend.Features.CartFeature.AddCartItem;

public record AddCartItemCommand(
    string? SessionId,
    long ProductId,
    long? VariantId = null,
    int Quantity = 1,
    long? UserId = null) : IRequest<Result<AddCartItemResponse>>;

public record AddCartItemResponse(CartResponse Cart, string SessionId);
