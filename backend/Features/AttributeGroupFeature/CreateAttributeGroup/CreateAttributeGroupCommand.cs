using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeGroupFeature.CreateAttributeGroup;

public record CreateAttributeGroupAttributeInput(
    string NameEn,
    string? NameAr = null,
    string? Slug = null,
    string? Unit = null,
    string InputType = "text", // text / select / boolean / number
    bool IsFilterable = true,
    int SortOrder = 0);

public record CreateAttributeGroupCommand(
    string NameEn,
    string? NameAr = null,
    string? Slug = null,
    int SortOrder = 0,
    bool IsFilterable = true,
    List<CreateAttributeGroupAttributeInput>? Attributes = null) : IRequest<Result<CreateAttributeGroupResponse>>;

public record CreateAttributeGroupResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    int SortOrder,
    bool IsFilterable,
    DateTime CreatedAt,
    List<CreateAttributeGroupAttributeResponse> Attributes);

public record CreateAttributeGroupAttributeResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    string? Unit,
    string InputType,
    bool IsFilterable,
    int SortOrder,
    DateTime CreatedAt);
