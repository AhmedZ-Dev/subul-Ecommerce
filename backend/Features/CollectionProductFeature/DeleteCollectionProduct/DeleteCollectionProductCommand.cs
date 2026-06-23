using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionProductFeature.DeleteCollectionProduct;

public record DeleteCollectionProductCommand(long CollectionId, long Id) : IRequest<Result<bool>>;
