using backend.Common.Results;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using MediatR;

namespace backend.Features.CollectionProductFeature.UpdateCollectionProduct;

public record UpdateCollectionProductCommand(
    long CollectionId,
    long Id,
    int SortOrder) : IRequest<Result<CollectionProductLinkResponse>>;

public record UpdateCollectionProductRequest(int SortOrder);
