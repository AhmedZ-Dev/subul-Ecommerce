using backend.Common.Results;
using MediatR;

namespace backend.Features.CategoryFeature.GetByIdCategory;

public record GetByIdCategoryQuery(long Id) : IRequest<Result<GetByIdCategoryResponse>>;

public record GetByIdCategoryResponse(
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
