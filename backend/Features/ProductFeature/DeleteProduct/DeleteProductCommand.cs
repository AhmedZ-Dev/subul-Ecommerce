using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductFeature.DeleteProduct;

public record DeleteProductCommand(long Id) : IRequest<Result<bool>>;
