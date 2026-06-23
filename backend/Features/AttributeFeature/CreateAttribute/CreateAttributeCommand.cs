using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeFeature.CreateAttribute;

public record CreateAttributeCommand(
    long GroupId,
    string NameEn,
    string? NameAr = null,
    string? Slug = null,
    string? Unit = null,
    string InputType = "text",
    bool IsFilterable = true,
    int SortOrder = 0) : IRequest<Result<AttributeResponse>>;

public record CreateAttributeRequest(
    string NameEn,
    string? NameAr = null,
    string? Slug = null,
    string? Unit = null,
    string InputType = "text",
    bool IsFilterable = true,
    int SortOrder = 0);

public record AttributeResponse(
    long Id,
    long? GroupId,
    string NameEn,
    string? NameAr,
    string Slug,
    string? Unit,
    string InputType,
    bool IsFilterable,
    int SortOrder,
    DateTime CreatedAt);
