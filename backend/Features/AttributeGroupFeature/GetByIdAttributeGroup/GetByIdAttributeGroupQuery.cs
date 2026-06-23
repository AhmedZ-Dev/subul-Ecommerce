using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeGroupFeature.GetByIdAttributeGroup;

public record GetByIdAttributeGroupQuery(long Id) : IRequest<Result<GetByIdAttributeGroupResponse>>;

public record GetByIdAttributeGroupResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    int SortOrder,
    bool IsFilterable,
    DateTime CreatedAt,
    List<GetByIdAttributeGroupAttributeResponse> Attributes);

public record GetByIdAttributeGroupAttributeResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    string? Unit,
    string InputType,
    bool IsFilterable,
    int SortOrder,
    DateTime CreatedAt);
