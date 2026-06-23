using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CartFeature.RemoveCartItem;

public class RemoveCartItemHandler(AppDbContext context)
    : IRequestHandler<RemoveCartItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        RemoveCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var sessionId = NormalizeSession(command.SessionId);
        if (sessionId is null)
            return Result<bool>.Failure("Cart session is required");

        var cartItem = await context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == command.CartItemId, cancellationToken);

        if (cartItem is null)
            return Result<bool>.Failure("Cart item not found");

        if (!BelongsToCaller(cartItem.Cart, sessionId, command.UserId))
            return Result<bool>.Failure("Cart item not found");

        cartItem.Cart.UpdatedAt = DateTime.Now;
        context.CartItems.Remove(cartItem);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    private static string? NormalizeSession(string? sessionId) =>
        string.IsNullOrWhiteSpace(sessionId) ? null : sessionId.Trim();

    private static bool BelongsToCaller(Cart cart, string sessionId, long? userId)
    {
        if (userId is not null && cart.UserId == userId)
            return true;

        return string.Equals(cart.SessionId, sessionId, StringComparison.Ordinal);
    }
}
