using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeGroupFeature.UpdateAttributeGroup;

public record UpdateAttributeGroupAttributeInput(
    long? Id,
    string NameEn,
    string? NameAr = null,
    string? Slug = null,
    string? Unit = null,
    string InputType = "text",
    bool IsFilterable = true,
    int SortOrder = 0);

public record UpdateAttributeGroupCommand(
    long Id,
    string NameEn,
    string? NameAr,
    string? Slug,
    int SortOrder,
    bool IsFilterable,
    List<UpdateAttributeGroupAttributeInput>? Attributes) : IRequest<Result<UpdateAttributeGroupResponse>>;

public record UpdateAttributeGroupRequest(
    string NameEn,
    string? NameAr,
    string? Slug,
    int SortOrder,
    bool IsFilterable,
    List<UpdateAttributeGroupAttributeInput>? Attributes);

public record UpdateAttributeGroupResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    int SortOrder,
    bool IsFilterable,
    DateTime CreatedAt,
    List<UpdateAttributeGroupAttributeResponse> Attributes);

public record UpdateAttributeGroupAttributeResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    string? Unit,
    string InputType,
    bool IsFilterable,
    int SortOrder,
    DateTime CreatedAt);
