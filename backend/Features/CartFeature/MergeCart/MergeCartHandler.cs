using backend.Common.Results;
using backend.Domain.Entities;
using backend.Features.CartFeature.GetCart;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CartFeature.MergeCart;

public class MergeCartHandler(AppDbContext context)
    : IRequestHandler<MergeCartCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(
        MergeCartCommand command,
        CancellationToken cancellationToken)
    {
        var sessionId = NormalizeSession(command.SessionId);
        if (sessionId is null)
            return Result<CartResponse>.Failure("Cart session is required");

        var userExists = await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
        if (!userExists)
            return Result<CartResponse>.Failure("User not found");

        var guestCart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.UserId == null, cancellationToken);

        var userCart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == command.UserId, cancellationToken);

        var now = DateTime.Now;

        if (guestCart is null)
        {
            var cart = await GetOrCreateCartAsync(context, sessionId, command.UserId, cancellationToken);
            var emptyResponse = await MapCartResponseAsync(context, cart, cancellationToken);
            return Result<CartResponse>.Success(emptyResponse);
        }

        if (userCart is null)
        {
            guestCart.UserId = command.UserId;
            guestCart.UpdatedAt = now;
            await context.SaveChangesAsync(cancellationToken);

            var response = await MapCartResponseAsync(context, guestCart, cancellationToken);
            return Result<CartResponse>.Success(response);
        }

        foreach (var guestItem in guestCart.CartItems.ToList())
        {
            var existing = userCart.CartItems.FirstOrDefault(ci =>
                ci.ProductId == guestItem.ProductId && ci.VariantId == guestItem.VariantId);

            if (existing is not null)
            {
                existing.Quantity += guestItem.Quantity;
                existing.UnitPrice = guestItem.UnitPrice;
                existing.UpdatedAt = now;
                context.CartItems.Remove(guestItem);
            }
            else
            {
                guestItem.CartId = userCart.Id;
                guestItem.UpdatedAt = now;
            }
        }

        userCart.SessionId = sessionId;
        userCart.UpdatedAt = now;
        context.Carts.Remove(guestCart);

        await context.SaveChangesAsync(cancellationToken);

        var mergedResponse = await MapCartResponseAsync(context, userCart, cancellationToken);
        return Result<CartResponse>.Success(mergedResponse);
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
                ci.Product.Slug,
                ci.Variant?.Sku ?? ci.Product.Sku,
                ci.Quantity,
                unitPrice,
                unitPrice * ci.Quantity);
        }).ToList();

        return new CartResponse(
            cart.Id,
            cart.SessionId ?? string.Empty,
            cart.UserId,
            cart.CouponCode,
            cart.Notes,
            mappedItems,
            mappedItems.Sum(i => i.LineTotal),
            mappedItems.Sum(i => i.Quantity));
    }
}
