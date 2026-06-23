using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductVariantFeature.DeleteProductVariant;

public record DeleteProductVariantCommand(long ProductId, long Id) : IRequest<Result<bool>>;
