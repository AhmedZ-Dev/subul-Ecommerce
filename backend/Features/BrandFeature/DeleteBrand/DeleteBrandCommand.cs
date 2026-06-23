using backend.Common.Results;
using MediatR;

namespace backend.Features.BrandFeature.DeleteBrand;

public record DeleteBrandCommand(long Id) : IRequest<Result<bool>>;
