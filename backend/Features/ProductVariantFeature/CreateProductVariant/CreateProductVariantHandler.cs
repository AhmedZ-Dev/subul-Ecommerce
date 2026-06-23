using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductVariantFeature.CreateProductVariant;

public class CreateProductVariantHandler(AppDbContext context)
    : IRequestHandler<CreateProductVariantCommand, Result<ProductVariantResponse>>
{
    public async Task<Result<ProductVariantResponse>> Handle(
        CreateProductVariantCommand command,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(
            p => p.Id == command.ProductId,
            cancellationToken);

        if (!productExists)
            return Result<ProductVariantResponse>.Failure("Product not found");

        var normalizedSku = command.Sku?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSku))
        {
            var skuExists = await context.ProductVariants.AnyAsync(
                v => v.Sku != null && v.Sku.ToLower() == normalizedSku.ToLower(),
                cancellationToken);

            if (skuExists)
                return Result<ProductVariantResponse>.Failure("Product variant SKU already exists");
        }

        var now = DateTime.Now;

        var variant = new ProductVariant
        {
            ProductId = command.ProductId,
            Title = command.Title?.Trim(),
            Sku = normalizedSku,
            Barcode = command.Barcode?.Trim(),
            Price = command.Price,
            CompareAtPrice = command.CompareAtPrice,
            CostPrice = command.CostPrice,
            StockQuantity = command.StockQuantity,
            Weight = command.Weight,
            IsActive = command.IsActive,
            SortOrder = command.SortOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.ProductVariants.Add(variant);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductVariantResponse>.Success(MapResponse(variant));
    }

    internal static ProductVariantResponse MapResponse(ProductVariant variant) =>
        new(
            variant.Id,
            variant.ProductId,
            variant.Title,
            variant.Sku,
            variant.Barcode,
            variant.Price,
            variant.CompareAtPrice,
            variant.CostPrice,
            variant.StockQuantity,
            variant.Weight,
            variant.IsActive,
            variant.SortOrder,
            variant.CreatedAt,
            variant.UpdatedAt);
}
