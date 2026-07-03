using backend.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace backend.Features.BrandFeature.UploadBrandLogo;

public record UploadBrandLogoCommand(long BrandId, IFormFile Image)
    : IRequest<Result<BrandImageAssetResponse>>;
