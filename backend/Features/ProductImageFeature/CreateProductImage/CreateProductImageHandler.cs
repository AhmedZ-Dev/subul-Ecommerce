using backend.Common.Results;
using backend.Common.Storage;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductImageFeature.CreateProductImage;

public class CreateProductImageHandler(AppDbContext context, IImageStorageService imageStorage)
    : IRequestHandler<CreateProductImageCommand, Result<ProductImageResponse>>
{
    public async Task<Result<ProductImageResponse>> Handle(
        CreateProductImageCommand command,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(
            p => p.Id == command.ProductId,
            cancellationToken);

        if (!productExists)
            return Result<ProductImageResponse>.Failure("Product not found");

        if (command.VariantId is not null)
        {
            var variantExists = await context.ProductVariants.AnyAsync(
                v => v.Id == command.VariantId && v.ProductId == command.ProductId,
                cancellationToken);

            if (!variantExists)
                return Result<ProductImageResponse>.Failure("Product variant not found");
        }

        var saveResult = await imageStorage.SaveProductImageAsync(
            command.ProductId,
            command.Image,
            cancellationToken);

        if (!saveResult.IsSuccess)
            return Result<ProductImageResponse>.Failure(saveResult.Error!);

        if (command.IsPrimary)
            await ClearOtherPrimaryImagesAsync(command.ProductId, null, cancellationToken);

        var now = DateTime.Now;
        var image = new ProductImage
        {
            ProductId = command.ProductId,
            VariantId = command.VariantId,
            ImageUrl = saveResult.Value!,
            AltText = command.AltText?.Trim(),
            SortOrder = command.SortOrder,
            IsPrimary = command.IsPrimary,
            CreatedAt = now
        };

        context.ProductImages.Add(image);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductImageResponse>.Success(MapResponse(image));
    }

    internal static ProductImageResponse MapResponse(ProductImage image) =>
        new(
            image.Id,
            image.ProductId,
            image.VariantId,
            image.ImageUrl,
            image.AltText,
            image.SortOrder,
            image.IsPrimary,
            image.CreatedAt);

    internal async Task ClearOtherPrimaryImagesAsync(
        long productId,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var others = await context.ProductImages
            .Where(pi => pi.ProductId == productId &&
                         pi.IsPrimary &&
                         (excludeId == null || pi.Id != excludeId))
            .ToListAsync(cancellationToken);

        foreach (var other in others)
            other.IsPrimary = false;
    }
}
