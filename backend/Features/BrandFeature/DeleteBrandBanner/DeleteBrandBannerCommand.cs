using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.DeleteBrandBanner;

public record DeleteBrandBannerCommand(long BrandId) : IRequest<Result<BrandImageAssetResponse>>;
