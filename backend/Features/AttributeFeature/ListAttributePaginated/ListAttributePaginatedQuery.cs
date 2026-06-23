using backend.Common.Results;
using backend.Features.AttributeFeature.CreateAttribute;
using MediatR;

namespace backend.Features.AttributeFeature.ListAttributePaginated;

public record ListAttributePaginatedQuery(
    long GroupId = 0,
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    bool? IsFilterable = null,
    string? SortBy = "sortOrder",
    string? SortOrder = "asc") : IRequest<Result<ListAttributePaginatedResponse>>;

public record ListAttributePaginatedResponse(
    List<AttributeResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);
