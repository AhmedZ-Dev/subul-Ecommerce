using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductFeature.UpdateProduct;

public class UpdateProductHandler(AppDbContext context)
    : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "active", "draft", "archived"
    };

    public async Task<Result<UpdateProductResponse>> Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (product is null)
            return Result<UpdateProductResponse>.Failure("Product not found");

        var normalizedNameEn = command.NameEn.Trim();
        var normalizedStatus = command.Status.Trim().ToLowerInvariant();

        if (!ValidStatuses.Contains(normalizedStatus))
            return Result<UpdateProductResponse>.Failure("Invalid product status");

        if (command.CategoryId is not null)
        {
            var categoryExists = await context.Categories.AnyAsync(
                c => c.Id == command.CategoryId,
                cancellationToken);

            if (!categoryExists)
                return Result<UpdateProductResponse>.Failure("Category not found");
        }

        if (command.BrandId is not null)
        {
            var brandExists = await context.Brands.AnyAsync(
                b => b.Id == command.BrandId,
                cancellationToken);

            if (!brandExists)
                return Result<UpdateProductResponse>.Failure("Brand not found");
        }

        var normalizedSku = command.Sku?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSku))
        {
            var skuExists = await context.Products.AnyAsync(
                p => p.Id != command.Id && p.Sku != null && p.Sku.ToLower() == normalizedSku.ToLower(),
                cancellationToken);

            if (skuExists)
                return Result<UpdateProductResponse>.Failure("Product SKU already exists");
        }

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, command.Id, cancellationToken);

        product.NameEn = normalizedNameEn;
        product.NameAr = command.NameAr?.Trim();
        product.CategoryId = command.CategoryId;
        product.BrandId = command.BrandId;
        product.Slug = slug;
        product.Sku = normalizedSku;
        product.Barcode = command.Barcode?.Trim();
        product.DescriptionEn = command.DescriptionEn;
        product.DescriptionAr = command.DescriptionAr;
        product.ShortDescriptionEn = command.ShortDescriptionEn;
        product.ShortDescriptionAr = command.ShortDescriptionAr;
        product.Price = command.Price;
        product.CompareAtPrice = command.CompareAtPrice;
        product.CostPrice = command.CostPrice;
        product.Currency = command.Currency.Trim();
        product.StockQuantity = command.StockQuantity;
        product.LowStockThreshold = command.LowStockThreshold;
        product.MinOrderQuantity = command.MinOrderQuantity;
        product.Weight = command.Weight;
        product.Status = normalizedStatus;
        product.IsFeatured = command.IsFeatured;
        product.RequiresShipping = command.RequiresShipping;
        product.WarrantyMonths = command.WarrantyMonths;
        product.WarrantyDescription = command.WarrantyDescription;
        product.MetaTitle = command.MetaTitle;
        product.MetaDescription = command.MetaDescription;
        product.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        Category? category = null;
        Brand? brand = null;

        if (product.CategoryId is not null)
        {
            category = await context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == product.CategoryId, cancellationToken);
        }

        if (product.BrandId is not null)
        {
            brand = await context.Brands.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == product.BrandId, cancellationToken);
        }

        var response = MapResponse(product, category, brand);
        return Result<UpdateProductResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueSlugAsync(
        string baseSlug,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.Products.AnyAsync(
                   p => p.Slug == slug && (excludeId == null || p.Id != excludeId),
                   cancellationToken))
        {
            suffix++;
            slug = $"{baseSlug}-{suffix}";
        }

        return slug;
    }

    private static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = InvalidSlugCharsRegex.Replace(slug, string.Empty);
        slug = WhitespaceRegex.Replace(slug, "-");
        slug = DuplicateHyphensRegex.Replace(slug, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "product" : slug;
    }

    private static UpdateProductResponse MapResponse(Product product, Category? category, Brand? brand)
    {
        return new UpdateProductResponse(
            product.Id,
            product.CategoryId,
            product.BrandId,
            product.NameEn,
            product.NameAr,
            product.Slug,
            product.Sku,
            product.Barcode,
            product.DescriptionEn,
            product.DescriptionAr,
            product.ShortDescriptionEn,
            product.ShortDescriptionAr,
            product.Price,
            product.CompareAtPrice,
            product.CostPrice,
            product.Currency,
            product.StockQuantity,
            product.LowStockThreshold,
            product.MinOrderQuantity,
            product.Weight,
            product.Status,
            product.IsFeatured,
            product.RequiresShipping,
            product.WarrantyMonths,
            product.WarrantyDescription,
            product.TotalSold,
            product.ViewsCount,
            product.MetaTitle,
            product.MetaDescription,
            product.CreatedAt,
            product.UpdatedAt,
            category is not null
                ? new ProductCategoryInfo(category.Id, category.NameEn, category.NameAr)
                : null,
            brand is not null
                ? new ProductBrandInfo(brand.Id, brand.Name, brand.Slug)
                : null);
    }
}
