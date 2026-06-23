using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductFeature.DeleteProduct;

public class DeleteProductHandler(AppDbContext context)
    : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (product is null)
            return Result<bool>.Failure("Product not found");

        var hasOrderItems = await context.OrderItems
            .AnyAsync(oi => oi.ProductId == command.Id, cancellationToken);

        if (hasOrderItems)
            return Result<bool>.Failure("Cannot delete product because it is associated with orders");

        var hasCartItems = await context.CartItems
            .AnyAsync(ci => ci.ProductId == command.Id, cancellationToken);

        if (hasCartItems)
            return Result<bool>.Failure("Cannot delete product because it is in shopping carts");

        var hasVariants = await context.ProductVariants
            .AnyAsync(v => v.ProductId == command.Id, cancellationToken);

        if (hasVariants)
            return Result<bool>.Failure("Cannot delete product because it has variants");

        var hasReviews = await context.Reviews
            .AnyAsync(r => r.ProductId == command.Id, cancellationToken);

        if (hasReviews)
            return Result<bool>.Failure("Cannot delete product because it has reviews");

        var hasImages = await context.ProductImages
            .AnyAsync(pi => pi.ProductId == command.Id, cancellationToken);

        if (hasImages)
            return Result<bool>.Failure("Cannot delete product because it has images");

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
