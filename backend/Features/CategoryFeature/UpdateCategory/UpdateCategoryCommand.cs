using backend.Common.Results;
using MediatR;

namespace backend.Features.CategoryFeature.UpdateCategory;

public record UpdateCategoryCommand(
    long Id,
    string NameEn,
    string? NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    long? ParentId,
    string? Slug,
    string? ImageUrl,
    string? BannerUrl,
    int SortOrder,
    bool IsActive,
    string? SeoTitle,
    string? SeoDescription) : IRequest<Result<UpdateCategoryResponse>>;

public record UpdateCategoryRequest(
    string NameEn,
    string? NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    long? ParentId,
    string? Slug,
    string? ImageUrl,
    string? BannerUrl,
    int SortOrder,
    bool IsActive,
    string? SeoTitle,
    string? SeoDescription);

public record UpdateCategoryResponse(
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
