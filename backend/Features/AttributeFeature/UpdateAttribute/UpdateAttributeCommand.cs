using backend.Common.Results;
using backend.Features.AttributeFeature.CreateAttribute;
using MediatR;

namespace backend.Features.AttributeFeature.UpdateAttribute;

public record UpdateAttributeCommand(
    long GroupId,
    long Id,
    string NameEn,
    string? NameAr,
    string? Slug,
    string? Unit,
    string InputType,
    bool IsFilterable,
    int SortOrder) : IRequest<Result<AttributeResponse>>;

public record UpdateAttributeRequest(
    string NameEn,
    string? NameAr,
    string? Slug,
    string? Unit,
    string InputType,
    bool IsFilterable,
    int SortOrder);
