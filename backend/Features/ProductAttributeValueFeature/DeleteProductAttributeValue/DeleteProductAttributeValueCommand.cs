using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductAttributeValueFeature.DeleteProductAttributeValue;

public record DeleteProductAttributeValueCommand(long ProductId, long Id) : IRequest<Result<bool>>;
