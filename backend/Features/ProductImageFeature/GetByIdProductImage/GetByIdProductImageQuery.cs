using backend.Common.Results;
using backend.Features.ProductImageFeature.CreateProductImage;
using MediatR;

namespace backend.Features.ProductImageFeature.GetByIdProductImage;

public record GetByIdProductImageQuery(long ProductId, long Id)
    : IRequest<Result<ProductImageResponse>>;
