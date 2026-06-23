using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.ListBrandPaginated;

public record ListBrandPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    bool? IsActive = null,
    bool? IsFeatured = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListBrandPaginatedResponse>>;

public record ListBrandPaginatedResponse(
    List<ListBrandPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListBrandPaginatedItemResponse(
    long Id,
    string Name,
    string Slug,
    string? LogoUrl,
    string? BannerUrl,
    string? DescriptionEn,
    string? DescriptionAr,
    string? WebsiteUrl,
    bool IsFeatured,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    BrandProductCountResponse _count);

public record BrandProductCountResponse(int Product);
