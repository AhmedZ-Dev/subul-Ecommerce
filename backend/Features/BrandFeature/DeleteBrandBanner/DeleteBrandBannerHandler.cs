using backend.Common.Results;
using backend.Common.Storage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.DeleteBrandBanner;

public class DeleteBrandBannerHandler(AppDbContext context, IImageStorageService imageStorage)
    : IRequestHandler<DeleteBrandBannerCommand, Result<BrandImageAssetResponse>>
{
    public async Task<Result<BrandImageAssetResponse>> Handle(
        DeleteBrandBannerCommand command,
        CancellationToken cancellationToken)
    {
        var brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.BrandId, cancellationToken);

        if (brand is null)
            return Result<BrandImageAssetResponse>.Failure("Brand not found");

        if (!string.IsNullOrEmpty(brand.BannerUrl) &&
            brand.BannerUrl.StartsWith("/img/brands/", StringComparison.OrdinalIgnoreCase))
        {
            await imageStorage.DeleteByRelativePathAsync(brand.BannerUrl, cancellationToken);
        }

        brand.BannerUrl = null;
        brand.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        return Result<BrandImageAssetResponse>.Success(new BrandImageAssetResponse(
            brand.Id,
            brand.LogoUrl,
            brand.BannerUrl,
            brand.UpdatedAt));
    }
}
