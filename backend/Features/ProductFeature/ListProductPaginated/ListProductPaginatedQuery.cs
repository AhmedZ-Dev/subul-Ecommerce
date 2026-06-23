using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductFeature.ListProductPaginated;

public record ListProductPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    long? CategoryId = null,
    long? BrandId = null,
    string? Status = null,
    bool? IsFeatured = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListProductPaginatedResponse>>;

public record ListProductPaginatedResponse(
    List<ListProductPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListProductPaginatedItemResponse(
    long Id,
    long? CategoryId,
    long? BrandId,
    string NameEn,
    string? NameAr,
    string Slug,
    string? Sku,
    string? Barcode,
    string? ShortDescriptionEn,
    string? ShortDescriptionAr,
    decimal Price,
    decimal? CompareAtPrice,
    string Currency,
    int StockQuantity,
    string Status,
    bool IsFeatured,
    int TotalSold,
    int ViewsCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    ProductCategoryInfo? Category,
    ProductBrandInfo? Brand);

public record ProductCategoryInfo(long Id, string NameEn, string? NameAr);

public record ProductBrandInfo(long Id, string Name, string Slug);
