using backend.Common.Results;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using MediatR;

namespace backend.Features.ProductVariantFeature.UpdateProductVariant;

public record UpdateProductVariantCommand(
    long ProductId,
    long Id,
    string? Title,
    string? Sku,
    string? Barcode,
    decimal? Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    int StockQuantity,
    decimal? Weight,
    bool IsActive,
    int SortOrder) : IRequest<Result<ProductVariantResponse>>;

public record UpdateProductVariantRequest(
    string? Title,
    string? Sku,
    string? Barcode,
    decimal? Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    int StockQuantity,
    decimal? Weight,
    bool IsActive,
    int SortOrder);
