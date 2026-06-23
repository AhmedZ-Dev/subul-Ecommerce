using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionProductFeature.CreateCollectionProduct;

public record CreateCollectionProductCommand(
    long CollectionId,
    long ProductId,
    int SortOrder = 0) : IRequest<Result<CollectionProductLinkResponse>>;

public record CreateCollectionProductRequest(
    long ProductId,
    int SortOrder = 0);

public record CollectionProductLinkResponse(
    long Id,
    long CollectionId,
    long ProductId,
    int SortOrder,
    DateTime CreatedAt,
    CollectionProductInfo Product);

public record CollectionProductInfo(
    string NameEn,
    string? NameAr,
    string Slug,
    decimal Price);
