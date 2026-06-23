using backend.Common.Results;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using MediatR;

namespace backend.Features.CollectionProductFeature.GetByIdCollectionProduct;

public record GetByIdCollectionProductQuery(long CollectionId, long Id)
    : IRequest<Result<CollectionProductLinkResponse>>;
