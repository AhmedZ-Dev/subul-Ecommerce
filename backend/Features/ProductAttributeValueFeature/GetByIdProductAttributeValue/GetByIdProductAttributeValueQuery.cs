using backend.Common.Results;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using MediatR;

namespace backend.Features.ProductAttributeValueFeature.GetByIdProductAttributeValue;

public record GetByIdProductAttributeValueQuery(long ProductId, long Id)
    : IRequest<Result<ProductAttributeValueResponse>>;
