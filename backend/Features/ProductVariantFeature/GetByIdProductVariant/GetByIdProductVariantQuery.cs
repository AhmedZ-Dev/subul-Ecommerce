using backend.Common.Results;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using MediatR;

namespace backend.Features.ProductVariantFeature.GetByIdProductVariant;

public record GetByIdProductVariantQuery(long ProductId, long Id)
    : IRequest<Result<ProductVariantResponse>>;
