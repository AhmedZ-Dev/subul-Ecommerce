using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AddressFeature.ListAddressPaginated;

public record ListAddressPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    long? UserId = null,
    string? Search = null,
    bool? IsDefault = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListAddressPaginatedResponse>>;

public record ListAddressPaginatedResponse(
    List<ListAddressPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListAddressPaginatedItemResponse(
    long Id,
    long UserId,
    string? FirstName,
    string? LastName,
    string? Phone,
    string Address1,
    string? Address2,
    string? City,
    string? Governorate,
    string Country,
    bool IsDefault,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
