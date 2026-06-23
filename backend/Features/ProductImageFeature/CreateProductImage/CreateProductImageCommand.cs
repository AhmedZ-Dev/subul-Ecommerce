using backend.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace backend.Features.ProductImageFeature.CreateProductImage;

public record CreateProductImageCommand(
    long ProductId,
    IFormFile Image,
    long? VariantId,
    string? AltText,
    int SortOrder,
    bool IsPrimary) : IRequest<Result<ProductImageResponse>>;

public record ProductImageResponse(
    long Id,
    long ProductId,
    long? VariantId,
    string ImageUrl,
    string? AltText,
    int SortOrder,
    bool IsPrimary,
    DateTime CreatedAt);
