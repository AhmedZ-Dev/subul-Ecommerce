using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionFeature.ListCollectionPaginated;

public record ListCollectionPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    bool? IsActive = null,
    string? CollectionType = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListCollectionPaginatedResponse>>;

public record ListCollectionPaginatedResponse(
    List<ListCollectionPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListCollectionPaginatedItemResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    string? DescriptionEn,
    string? DescriptionAr,
    string? ImageUrl,
    string? BannerUrl,
    string CollectionType,
    bool IsActive,
    int SortOrder,
    string? MetaTitle,
    string? MetaDescription,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int ProductCount);
