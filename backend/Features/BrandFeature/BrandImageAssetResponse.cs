namespace backend.Features.BrandFeature;

public record BrandImageAssetResponse(
    long Id,
    string? LogoUrl,
    string? BannerUrl,
    DateTime? UpdatedAt);
