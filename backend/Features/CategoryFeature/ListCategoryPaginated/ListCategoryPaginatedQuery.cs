using backend.Common.Results;
using MediatR;

namespace backend.Features.CategoryFeature.ListCategoryPaginated;

public record ListCategoryPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    long? ParentId = null,
    bool? IsActive = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListCategoryPaginatedResponse>>;

public record ListCategoryPaginatedResponse(
    List<ListCategoryPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListCategoryPaginatedItemResponse(
    long Id,
    long? ParentId,
    string NameEn,
    string? NameAr,
    string Slug,
    string? DescriptionEn,
    string? DescriptionAr,
    string? ImageUrl,
    string? BannerUrl,
    int SortOrder,
    bool IsActive,
    string? SeoTitle,
    string? SeoDescription,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    CategoryParentInfo? Parent,
    CategoryProductCountResponse _count);

public record CategoryParentInfo(long Id, string NameEn, string? NameAr);

public record CategoryProductCountResponse(int Product, int SubCategory);
