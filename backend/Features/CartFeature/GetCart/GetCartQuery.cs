using backend.Common.Results;
using MediatR;

namespace backend.Features.CartFeature.GetCart;

public record GetCartQuery(string SessionId) : IRequest<Result<CartResponse>>;

public record CartItemResponse(
    long Id,
    long ProductId,
    long? VariantId,
    string ProductNameEn,
    string? ProductNameAr,
    string ProductSlug,
    string? Sku,
    string? ImageUrl,
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
