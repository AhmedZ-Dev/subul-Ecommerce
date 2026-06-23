using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.ShippingZoneFeature.ListShippingZonePaginated;

public record ListShippingZonePaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    bool? IsActive = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListShippingZonePaginatedResponse>>;

public record ListShippingZonePaginatedResponse(
    List<ListShippingZonePaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListShippingZonePaginatedItemResponse(
    long Id,
    string NameEn,
    string? NameAr,
    List<string> Governorates,
    bool IsActive,
    DateTime CreatedAt,
    int ShippingRateCount);
