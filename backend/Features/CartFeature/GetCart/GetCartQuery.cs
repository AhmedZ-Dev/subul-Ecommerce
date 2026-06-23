using backend.Common.Results;
using MediatR;

namespace backend.Features.CartFeature.GetCart;

public record GetCartQuery(string SessionId, long? UserId = null) : IRequest<Result<CartResponse>>;

public record CartItemResponse(
    long Id,
    long ProductId,
    long? VariantId,
    string ProductNameEn,
    string? ProductNameAr,
    string? Sku,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public record CartResponse(
    long Id,
    string SessionId,
    long? UserId,
    string? CouponCode,
    string? Notes,
    IReadOnlyList<CartItemResponse> Items,
    decimal Subtotal,
    int ItemCount);
