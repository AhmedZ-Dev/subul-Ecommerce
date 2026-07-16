using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductFeature.GetByIdProduct;

public record GetByIdProductQuery(long Id) : IRequest<Result<GetByIdProductResponse>>;

public record GetByIdProductResponse(
    long Id,
    long? CategoryId,
    long? BrandId,
    string NameEn,
    string? NameAr,
    string Slug,
    string? Sku,
    string? Barcode,
    string? DescriptionEn,
    string? DescriptionAr,
    string? ShortDescriptionEn,
    string? ShortDescriptionAr,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    string Currency,
    int StockQuantity,
    int LowStockThreshold,
    int MinOrderQuantity,
    decimal? Weight,
    string Status,
    bool IsFeatured,
    bool RequiresShipping,
    int WarrantyMonths,
    string? WarrantyDescription,
    int TotalSold,
    int ViewsCount,
    string? MetaTitle,
    string? MetaDescription,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    ProductCategoryInfo? Category,
    ProductBrandInfo? Brand,
    List<ProductVariantInfo> Variants,
    List<ProductAttributeValueInfo> AttributeValues);

public record ProductCategoryInfo(long Id, string NameEn, string? NameAr, string Slug);

public record ProductBrandInfo(long Id, string Name, string Slug);

public record ProductVariantInfo(
    long Id,
    string? Title,
    string? Sku,
    string? Barcode,
    decimal? Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    int StockQuantity,
    decimal? Weight,
    bool IsActive,
    int SortOrder);

public record ProductAttributeValueInfo(
    long Id,
    long AttributeId,
    string? ValueText,
    decimal? ValueNumber,
    bool? ValueBoolean,
    ProductAttributeValueAttributeInfo Attribute);

public record ProductAttributeValueAttributeInfo(
    string NameEn,
    string? NameAr,
    string? Unit,
    string InputType,
    int SortOrder);
