using backend.Common.Results;
using MediatR;
using backend.Features.ProductFeature.GetByIdProduct;

namespace backend.Features.ProductFeature.GetBySlugProduct;

public record GetBySlugProductQuery(string Slug, bool PublicOnly = false) : IRequest<Result<GetByIdProductResponse>>;
