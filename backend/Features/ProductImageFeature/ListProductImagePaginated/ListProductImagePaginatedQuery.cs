using backend.Common.Results;
using backend.Features.ProductImageFeature.CreateProductImage;
using MediatR;

namespace backend.Features.ProductImageFeature.ListProductImagePaginated;

public record ListProductImagePaginatedQuery(
    long ProductId = 0,
    int Page = 1,
    int Limit = 10,
    long? VariantId = null,
    string? SortBy = "sortOrder",
    string? SortOrder = "asc") : IRequest<Result<ListProductImagePaginatedResponse>>;

public record ListProductImagePaginatedResponse(
    List<ProductImageResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);
