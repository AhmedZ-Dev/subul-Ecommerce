using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductFeature.CreateProduct;

public record CreateProductCommand(
    string NameEn,
    string? NameAr,
    long? CategoryId,
    long? BrandId,
    string? Slug = null,
    string? Sku = null,
    string? Barcode = null,
    string? DescriptionEn = null,
    string? DescriptionAr = null,
    string? ShortDescriptionEn = null,
    string? ShortDescriptionAr = null,
    decimal Price = 0,
    decimal? CompareAtPrice = null,
    decimal? CostPrice = null,
    string Currency = "IQD",
    int StockQuantity = 0,
    int LowStockThreshold = 2,
    int MinOrderQuantity = 1,
    decimal? Weight = null,
    string Status = "active",
    bool IsFeatured = false,
    bool RequiresShipping = true,
    int WarrantyMonths = 12,
    string? WarrantyDescription = null,
    string? MetaTitle = null,
    string? MetaDescription = null) : IRequest<Result<CreateProductResponse>>;

public record CreateProductResponse(
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
    ProductBrandInfo? Brand);

public record ProductCategoryInfo(long Id, string NameEn, string? NameAr);

public record ProductBrandInfo(long Id, string Name, string Slug);
