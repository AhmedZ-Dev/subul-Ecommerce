using backend.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace backend.Features.BrandFeature.UploadBrandBanner;

public record UploadBrandBannerCommand(long BrandId, IFormFile Image)
    : IRequest<Result<BrandImageAssetResponse>>;
