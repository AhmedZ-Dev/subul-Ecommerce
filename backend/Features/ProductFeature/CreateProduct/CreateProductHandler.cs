using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductFeature.CreateProduct;

public class CreateProductHandler(AppDbContext context)
    : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "active", "draft", "archived"
    };

    public async Task<Result<CreateProductResponse>> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedNameEn = command.NameEn.Trim();
        var normalizedStatus = command.Status.Trim().ToLowerInvariant();

        if (!ValidStatuses.Contains(normalizedStatus))
            return Result<CreateProductResponse>.Failure("Invalid product status");

        if (command.CategoryId is not null)
        {
            var categoryExists = await context.Categories.AnyAsync(
                c => c.Id == command.CategoryId,
                cancellationToken);

            if (!categoryExists)
                return Result<CreateProductResponse>.Failure("Category not found");
        }

        if (command.BrandId is not null)
        {
            var brandExists = await context.Brands.AnyAsync(
                b => b.Id == command.BrandId,
                cancellationToken);

            if (!brandExists)
                return Result<CreateProductResponse>.Failure("Brand not found");
        }

        var normalizedSku = command.Sku?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSku))
        {
            var skuExists = await context.Products.AnyAsync(
                p => p.Sku != null && p.Sku.ToLower() == normalizedSku.ToLower(),
                cancellationToken);

            if (skuExists)
                return Result<CreateProductResponse>.Failure("Product SKU already exists");
        }

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, null, cancellationToken);
        var now = DateTime.Now;

        var product = new Product
        {
            NameEn = normalizedNameEn,
            NameAr = command.NameAr?.Trim(),
            CategoryId = command.CategoryId,
            BrandId = command.BrandId,
            Slug = slug,
            Sku = normalizedSku,
            Barcode = command.Barcode?.Trim(),
            DescriptionEn = command.DescriptionEn,
            DescriptionAr = command.DescriptionAr,
            ShortDescriptionEn = command.ShortDescriptionEn,
            ShortDescriptionAr = command.ShortDescriptionAr,
            Price = command.Price,
            CompareAtPrice = command.CompareAtPrice,
            CostPrice = command.CostPrice,
            Currency = command.Currency.Trim(),
            StockQuantity = command.StockQuantity,
            LowStockThreshold = command.LowStockThreshold,
            MinOrderQuantity = command.MinOrderQuantity,
            Weight = command.Weight,
            Status = normalizedStatus,
            IsFeatured = command.IsFeatured,
            RequiresShipping = command.RequiresShipping,
            WarrantyMonths = command.WarrantyMonths,
            WarrantyDescription = command.WarrantyDescription,
            MetaTitle = command.MetaTitle,
            MetaDescription = command.MetaDescription,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Products.Add(product);
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
        return Result<CreateProductResponse>.Success(response);
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

    private static CreateProductResponse MapResponse(Product product, Category? category, Brand? brand)
    {
        return new CreateProductResponse(
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
