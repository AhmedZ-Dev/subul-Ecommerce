using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.GetByIdBrand;

public record GetByIdBrandQuery(long Id) : IRequest<Result<GetByIdBrandResponse>>;

public record GetByIdBrandResponse(
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
