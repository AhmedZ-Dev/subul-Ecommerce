using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.DeleteBrandLogo;

public record DeleteBrandLogoCommand(long BrandId) : IRequest<Result<BrandImageAssetResponse>>;
