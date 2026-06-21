using backend.Common.Results;
using MediatR;

namespace backend.Features.CategoryFeature.CreateCategory;

public record CreateCategoryCommand(
    string NameEn,
    string? NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    long? ParentId,
    string? Slug = null,
    string? ImageUrl = null,
    string? BannerUrl = null,
    int SortOrder = 0,
    bool IsActive = true,
    string? SeoTitle = null,
    string? SeoDescription = null) : IRequest<Result<CreateCategoryResponse>>;

public record CreateCategoryResponse(
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
    CategoryProductCountResponse _count);

public record CategoryProductCountResponse(int Product, int SubCategory);
