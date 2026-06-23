using backend.Common.Results;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using MediatR;

namespace backend.Features.ProductAttributeValueFeature.UpdateProductAttributeValue;

public record UpdateProductAttributeValueCommand(
    long ProductId,
    long Id,
    long AttributeId,
    string? ValueText,
    decimal? ValueNumber,
    bool? ValueBoolean) : IRequest<Result<ProductAttributeValueResponse>>;

public record UpdateProductAttributeValueRequest(
    long AttributeId,
    string? ValueText,
    decimal? ValueNumber,
    bool? ValueBoolean);
