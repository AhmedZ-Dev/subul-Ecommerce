using backend.Common.Results;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductImageFeature.UpdateProductImage;

public class UpdateProductImageHandler(AppDbContext context)
    : IRequestHandler<UpdateProductImageCommand, Result<ProductImageResponse>>
{
    public async Task<Result<ProductImageResponse>> Handle(
        UpdateProductImageCommand command,
        CancellationToken cancellationToken)
    {
        var image = await context.ProductImages
            .FirstOrDefaultAsync(
                pi => pi.Id == command.Id && pi.ProductId == command.ProductId,
                cancellationToken);

        if (image is null)
            return Result<ProductImageResponse>.Failure("Product image not found");

        if (command.VariantId is not null)
        {
            var variantExists = await context.ProductVariants.AnyAsync(
                v => v.Id == command.VariantId && v.ProductId == command.ProductId,
                cancellationToken);

            if (!variantExists)
                return Result<ProductImageResponse>.Failure("Product variant not found");
        }

        if (command.IsPrimary)
        {
            var others = await context.ProductImages
                .Where(pi => pi.ProductId == command.ProductId &&
                             pi.IsPrimary &&
                             pi.Id != command.Id)
                .ToListAsync(cancellationToken);

            foreach (var other in others)
                other.IsPrimary = false;
        }

        image.VariantId = command.VariantId;
        image.AltText = command.AltText?.Trim();
        image.SortOrder = command.SortOrder;
        image.IsPrimary = command.IsPrimary;

        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductImageResponse>.Success(CreateProductImageHandler.MapResponse(image));
    }
}
