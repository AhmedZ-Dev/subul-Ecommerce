using backend.Common.Results;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using MediatR;

namespace backend.Features.ProductAttributeValueFeature.ListProductAttributeValuePaginated;

public record ListProductAttributeValuePaginatedQuery(
    long ProductId = 0,
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    long? AttributeId = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListProductAttributeValuePaginatedResponse>>;

public record ListProductAttributeValuePaginatedResponse(
    List<ProductAttributeValueResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);
