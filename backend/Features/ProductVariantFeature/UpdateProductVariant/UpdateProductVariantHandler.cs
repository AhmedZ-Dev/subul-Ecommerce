using backend.Common.Results;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductVariantFeature.UpdateProductVariant;

public class UpdateProductVariantHandler(AppDbContext context)
    : IRequestHandler<UpdateProductVariantCommand, Result<ProductVariantResponse>>
{
    public async Task<Result<ProductVariantResponse>> Handle(
        UpdateProductVariantCommand command,
        CancellationToken cancellationToken)
    {
        var variant = await context.ProductVariants
            .FirstOrDefaultAsync(
                v => v.Id == command.Id && v.ProductId == command.ProductId,
                cancellationToken);

        if (variant is null)
            return Result<ProductVariantResponse>.Failure("Product variant not found");

        var normalizedSku = command.Sku?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSku))
        {
            var skuExists = await context.ProductVariants.AnyAsync(
                v => v.Id != command.Id &&
                     v.Sku != null &&
                     v.Sku.ToLower() == normalizedSku.ToLower(),
                cancellationToken);

            if (skuExists)
                return Result<ProductVariantResponse>.Failure("Product variant SKU already exists");
        }

        variant.Title = command.Title?.Trim();
        variant.Sku = normalizedSku;
        variant.Barcode = command.Barcode?.Trim();
        variant.Price = command.Price;
        variant.CompareAtPrice = command.CompareAtPrice;
        variant.CostPrice = command.CostPrice;
        variant.StockQuantity = command.StockQuantity;
        variant.Weight = command.Weight;
        variant.IsActive = command.IsActive;
        variant.SortOrder = command.SortOrder;
        variant.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductVariantResponse>.Success(CreateProductVariantHandler.MapResponse(variant));
    }
}
