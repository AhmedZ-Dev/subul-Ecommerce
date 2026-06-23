using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AttributeGroupFeature.ListAttributeGroupPaginated;

public record ListAttributeGroupPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    bool? IsFilterable = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListAttributeGroupPaginatedResponse>>;

public record ListAttributeGroupPaginatedResponse(
    List<ListAttributeGroupPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListAttributeGroupPaginatedItemResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    int SortOrder,
    bool IsFilterable,
    DateTime CreatedAt,
    int AttributeCount);
