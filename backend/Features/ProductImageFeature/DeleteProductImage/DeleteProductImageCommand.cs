using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductImageFeature.DeleteProductImage;

public record DeleteProductImageCommand(long ProductId, long Id) : IRequest<Result<bool>>;
