using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;

public record CreateProductAttributeValueCommand(
    long ProductId,
    long AttributeId,
    string? ValueText = null,
    decimal? ValueNumber = null,
    bool? ValueBoolean = null) : IRequest<Result<ProductAttributeValueResponse>>;

public record CreateProductAttributeValueRequest(
    long AttributeId,
    string? ValueText = null,
    decimal? ValueNumber = null,
    bool? ValueBoolean = null);

public record ProductAttributeValueResponse(
    long Id,
    long ProductId,
    long AttributeId,
    string? ValueText,
    decimal? ValueNumber,
    bool? ValueBoolean,
    DateTime CreatedAt,
    ProductAttributeValueAttributeInfo Attribute);

public record ProductAttributeValueAttributeInfo(
    string NameEn,
    string? NameAr,
    string? Unit,
    string InputType);
