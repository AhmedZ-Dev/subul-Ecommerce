using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CartFeature.GetCart;

public class GetCartHandler(AppDbContext context)
    : IRequestHandler<GetCartQuery, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(
        GetCartQuery query,
        CancellationToken cancellationToken)
    {
        var sessionId = NormalizeSession(query.SessionId);
        if (sessionId is null)
            return Result<CartResponse>.Failure("Cart session is required");

        var cart = await GetOrCreateCartAsync(context, sessionId, query.UserId, cancellationToken);
        var response = await MapCartResponseAsync(context, cart, cancellationToken);
        return Result<CartResponse>.Success(response);
    }

    private static string? NormalizeSession(string? sessionId) =>
        string.IsNullOrWhiteSpace(sessionId) ? null : sessionId.Trim();

    private static async Task<Cart> GetOrCreateCartAsync(
        AppDbContext context,
        string sessionId,
        long? userId,
        CancellationToken cancellationToken)
    {
        Cart? cart = null;

        if (userId is not null)
        {
            cart = await context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }

        cart ??= await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);

        if (cart is not null)
        {
            if (userId is not null && cart.UserId is null)
            {
                cart.UserId = userId;
                cart.UpdatedAt = DateTime.Now;
            }

            if (cart.SessionId != sessionId)
            {
                cart.SessionId = sessionId;
                cart.UpdatedAt = DateTime.Now;
            }

            return cart;
        }

        var now = DateTime.Now;
        cart = new Cart
        {
            UserId = userId,
            SessionId = sessionId,
            ExpiresAt = now.AddDays(30),
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Carts.Add(cart);
        await context.SaveChangesAsync(cancellationToken);

        return cart;
    }

    private static async Task<CartResponse> MapCartResponseAsync(
        AppDbContext context,
        Cart cart,
        CancellationToken cancellationToken)
    {
        var items = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == cart.Id)
            .Include(ci => ci.Product)
            .Include(ci => ci.Variant)
            .OrderBy(ci => ci.Id)
            .ToListAsync(cancellationToken);

        var mappedItems = items.Select(ci =>
        {
            var unitPrice = ci.UnitPrice ?? ci.Variant?.Price ?? ci.Product.Price;
            return new CartItemResponse(
                ci.Id,
                ci.ProductId,
                ci.VariantId,
                ci.Product.NameEn,
                ci.Product.NameAr,
                ci.Variant?.Sku ?? ci.Product.Sku,
                ci.Quantity,
                unitPrice,
                unitPrice * ci.Quantity);
        }).ToList();

        var subtotal = mappedItems.Sum(i => i.LineTotal);
        var itemCount = mappedItems.Sum(i => i.Quantity);

        return new CartResponse(
            cart.Id,
            cart.SessionId ?? string.Empty,
            cart.UserId,
            cart.CouponCode,
            cart.Notes,
            mappedItems,
            subtotal,
            itemCount);
    }
}
