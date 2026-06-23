using backend.Common.Results;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using MediatR;

namespace backend.Features.CollectionProductFeature.ListCollectionProductPaginated;

public record ListCollectionProductPaginatedQuery(
    long CollectionId = 0,
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    string? SortBy = "sortOrder",
    string? SortOrder = "asc") : IRequest<Result<ListCollectionProductPaginatedResponse>>;

public record ListCollectionProductPaginatedResponse(
    List<CollectionProductLinkResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);
