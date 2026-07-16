using backend.Common.Results;
using backend.Features.ProductFeature.GetByIdProduct;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductFeature.GetBySlugProduct;

public class GetBySlugProductHandler(AppDbContext context)
    : IRequestHandler<GetBySlugProductQuery, Result<GetByIdProductResponse>>
{
    public async Task<Result<GetByIdProductResponse>> Handle(
        GetBySlugProductQuery query,
        CancellationToken cancellationToken)
    {
        var slug = query.Slug.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug))
            return Result<GetByIdProductResponse>.Failure("Product not found");

        var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductAttributeValues)
                .ThenInclude(pav => pav.Attribute)
            .FirstOrDefaultAsync(p => p.Slug.ToLower() == slug, cancellationToken);

        if (product is null)
            return Result<GetByIdProductResponse>.Failure("Product not found");

        ProductCategoryInfo? categoryInfo = product.Category is not null
            ? new ProductCategoryInfo(product.Category.Id, product.Category.NameEn, product.Category.NameAr, product.Category.Slug)
            : null;

        ProductBrandInfo? brandInfo = product.Brand is not null
            ? new ProductBrandInfo(product.Brand.Id, product.Brand.Name, product.Brand.Slug)
            : null;

        var variants = product.ProductVariants
            .OrderBy(v => v.SortOrder)
            .ThenBy(v => v.Id)
            .Select(v => new ProductVariantInfo(
                v.Id,
                v.Title,
                v.Sku,
                v.Barcode,
                v.Price,
                v.CompareAtPrice,
                v.CostPrice,
                v.StockQuantity,
                v.Weight,
                v.IsActive,
                v.SortOrder))
            .ToList();

        var attributeValues = product.ProductAttributeValues
            .OrderBy(pav => pav.Attribute.SortOrder)
            .ThenBy(pav => pav.AttributeId)
            .Select(pav => new ProductAttributeValueInfo(
                pav.Id,
                pav.AttributeId,
                pav.ValueText,
                pav.ValueNumber,
                pav.ValueBoolean,
                new ProductAttributeValueAttributeInfo(
                    pav.Attribute.NameEn,
                    pav.Attribute.NameAr,
                    pav.Attribute.Unit,
                    pav.Attribute.InputType,
                    pav.Attribute.SortOrder)))
            .ToList();

        var response = new GetByIdProductResponse(
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
            categoryInfo,
            brandInfo,
            variants,
            attributeValues);

        return Result<GetByIdProductResponse>.Success(response);
    }
}
