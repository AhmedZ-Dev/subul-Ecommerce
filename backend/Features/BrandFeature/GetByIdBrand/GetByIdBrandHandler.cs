using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.GetByIdBrand;

public class GetByIdBrandHandler(AppDbContext context)
    : IRequestHandler<GetByIdBrandQuery, Result<GetByIdBrandResponse>>
{
    public async Task<Result<GetByIdBrandResponse>> Handle(
        GetByIdBrandQuery query,
        CancellationToken cancellationToken)
    {
        var brand = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == query.Id, cancellationToken);

        if (brand is null)
            return Result<GetByIdBrandResponse>.Failure("Brand not found");

        var productCount = await context.Products.CountAsync(
            p => p.BrandId == brand.Id,
            cancellationToken);

        var response = new GetByIdBrandResponse(
            brand.Id,
            brand.Name,
            brand.Slug,
            brand.LogoUrl,
            brand.BannerUrl,
            brand.DescriptionEn,
            brand.DescriptionAr,
            brand.WebsiteUrl,
            brand.IsFeatured,
            brand.IsActive,
            brand.SortOrder,
            brand.CreatedAt,
            brand.UpdatedAt,
            new BrandProductCountResponse(productCount));

        return Result<GetByIdBrandResponse>.Success(response);
    }
}
