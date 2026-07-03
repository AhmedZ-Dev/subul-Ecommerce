using backend.Common.Results;
using backend.Common.Storage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.DeleteBrand;

public class DeleteBrandHandler(AppDbContext context, IImageStorageService imageStorage)
    : IRequestHandler<DeleteBrandCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteBrandCommand command,
        CancellationToken cancellationToken)
    {
        var brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (brand is null)
            return Result<bool>.Failure("Brand not found");

        var hasRelatedProducts = await context.Products
            .AnyAsync(p => p.BrandId == command.Id, cancellationToken);

        if (hasRelatedProducts)
            return Result<bool>.Failure("Cannot delete brand because it is associated with products");

        if (!string.IsNullOrEmpty(brand.LogoUrl) &&
            brand.LogoUrl.StartsWith("/img/brands/", StringComparison.OrdinalIgnoreCase))
        {
            await imageStorage.DeleteByRelativePathAsync(brand.LogoUrl, cancellationToken);
        }

        if (!string.IsNullOrEmpty(brand.BannerUrl) &&
            brand.BannerUrl.StartsWith("/img/brands/", StringComparison.OrdinalIgnoreCase))
        {
            await imageStorage.DeleteByRelativePathAsync(brand.BannerUrl, cancellationToken);
        }

        context.Brands.Remove(brand);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
