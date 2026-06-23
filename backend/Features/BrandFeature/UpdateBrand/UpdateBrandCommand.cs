using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.UpdateBrand;

public record UpdateBrandCommand(
    long Id,
    string Name,
    string? Slug,
    string? LogoUrl,
    string? BannerUrl,
    string? DescriptionEn,
    string? DescriptionAr,
    string? WebsiteUrl,
    bool IsFeatured,
    bool IsActive,
    int SortOrder) : IRequest<Result<UpdateBrandResponse>>;

public record UpdateBrandRequest(
    string Name,
    string? Slug,
    string? LogoUrl,
    string? BannerUrl,
    string? DescriptionEn,
    string? DescriptionAr,
    string? WebsiteUrl,
    bool IsFeatured,
    bool IsActive,
    int SortOrder);

public record UpdateBrandResponse(
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
