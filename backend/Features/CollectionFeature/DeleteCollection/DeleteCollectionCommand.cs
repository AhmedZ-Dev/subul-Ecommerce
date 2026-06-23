using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionFeature.DeleteCollection;

public record DeleteCollectionCommand(long Id) : IRequest<Result<bool>>;
