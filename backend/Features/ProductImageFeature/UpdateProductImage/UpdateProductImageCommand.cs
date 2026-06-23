using backend.Common.Results;
using backend.Features.ProductImageFeature.CreateProductImage;
using MediatR;

namespace backend.Features.ProductImageFeature.UpdateProductImage;

public record UpdateProductImageCommand(
    long ProductId,
    long Id,
    long? VariantId,
    string? AltText,
    int SortOrder,
    bool IsPrimary) : IRequest<Result<ProductImageResponse>>;

public record UpdateProductImageRequest(
    long? VariantId,
    string? AltText,
    int SortOrder,
    bool IsPrimary);
