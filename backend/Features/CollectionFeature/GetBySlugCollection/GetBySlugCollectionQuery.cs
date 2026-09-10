using backend.Common.Results;
using MediatR;
using backend.Features.CollectionFeature.GetByIdCollection;

namespace backend.Features.CollectionFeature.GetBySlugCollection;

public record GetBySlugCollectionQuery(string Slug, bool PublicOnly = false) : IRequest<Result<GetByIdCollectionResponse>>;
