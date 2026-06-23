using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductVariantFeature.DeleteProductVariant;

public class DeleteProductVariantHandler(AppDbContext context)
    : IRequestHandler<DeleteProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProductVariantCommand command,
        CancellationToken cancellationToken)
    {
        var variant = await context.ProductVariants
            .FirstOrDefaultAsync(
                v => v.Id == command.Id && v.ProductId == command.ProductId,
                cancellationToken);

        if (variant is null)
            return Result<bool>.Failure("Product variant not found");

        var hasOrderItems = await context.OrderItems
            .AnyAsync(oi => oi.VariantId == command.Id, cancellationToken);

        if (hasOrderItems)
            return Result<bool>.Failure("Cannot delete product variant because it is associated with orders");

        var hasCartItems = await context.CartItems
            .AnyAsync(ci => ci.VariantId == command.Id, cancellationToken);

        if (hasCartItems)
            return Result<bool>.Failure("Cannot delete product variant because it is in shopping carts");

        var hasInventoryMovements = await context.InventoryMovements
            .AnyAsync(im => im.VariantId == command.Id, cancellationToken);

        if (hasInventoryMovements)
            return Result<bool>.Failure("Cannot delete product variant because it has inventory movements");

        var hasProductImages = await context.ProductImages
            .AnyAsync(pi => pi.VariantId == command.Id, cancellationToken);

        if (hasProductImages)
            return Result<bool>.Failure("Cannot delete product variant because it has associated images");

        context.ProductVariants.Remove(variant);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
