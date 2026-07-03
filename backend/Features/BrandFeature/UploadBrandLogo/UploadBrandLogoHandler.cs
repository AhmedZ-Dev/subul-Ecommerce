using backend.Common.Results;
using backend.Common.Storage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.UploadBrandLogo;

public class UploadBrandLogoHandler(AppDbContext context, IImageStorageService imageStorage)
    : IRequestHandler<UploadBrandLogoCommand, Result<BrandImageAssetResponse>>
{
    public async Task<Result<BrandImageAssetResponse>> Handle(
        UploadBrandLogoCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Image is null || command.Image.Length == 0)
            return Result<BrandImageAssetResponse>.Failure("Image file is required");

        var brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.BrandId, cancellationToken);

        if (brand is null)
            return Result<BrandImageAssetResponse>.Failure("Brand not found");

        var previousPath = brand.LogoUrl;
        var isManagedPrevious = !string.IsNullOrEmpty(previousPath) &&
            previousPath.StartsWith("/img/brands/", StringComparison.OrdinalIgnoreCase);

        var saveResult = await imageStorage.SaveBrandImageAsync(
            command.BrandId,
            "logo",
            command.Image,
            cancellationToken);

        if (!saveResult.IsSuccess)
            return Result<BrandImageAssetResponse>.Failure(saveResult.Error!);

        var newPath = saveResult.Value!;
        brand.LogoUrl = newPath;
        brand.UpdatedAt = DateTime.Now;

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await imageStorage.DeleteByRelativePathAsync(newPath, cancellationToken);
            throw;
        }

        if (isManagedPrevious &&
            !string.Equals(previousPath, newPath, StringComparison.OrdinalIgnoreCase))
        {
            await imageStorage.DeleteByRelativePathAsync(previousPath!, cancellationToken);
        }

        return Result<BrandImageAssetResponse>.Success(new BrandImageAssetResponse(
            brand.Id,
            brand.LogoUrl,
            brand.BannerUrl,
            brand.UpdatedAt));
    }
}
