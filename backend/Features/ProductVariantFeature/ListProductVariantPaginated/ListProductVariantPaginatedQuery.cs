using backend.Common.Results;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using MediatR;

namespace backend.Features.ProductVariantFeature.ListProductVariantPaginated;

public record ListProductVariantPaginatedQuery(
    long ProductId = 0,
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    bool? IsActive = null,
    string? SortBy = "sortOrder",
    string? SortOrder = "asc") : IRequest<Result<ListProductVariantPaginatedResponse>>;

public record ListProductVariantPaginatedResponse(
    List<ProductVariantResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);
