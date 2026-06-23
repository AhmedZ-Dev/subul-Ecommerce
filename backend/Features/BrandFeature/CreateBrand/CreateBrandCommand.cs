using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.CreateBrand;

public record CreateBrandCommand(
    string Name,
    string? Slug = null,
    string? LogoUrl = null,
    string? BannerUrl = null,
    string? DescriptionEn = null,
    string? DescriptionAr = null,
    string? WebsiteUrl = null,
    bool IsFeatured = false,
    bool IsActive = true,
    int SortOrder = 0) : IRequest<Result<CreateBrandResponse>>;

public record CreateBrandResponse(
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
