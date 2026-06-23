using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductVariantFeature.CreateProductVariant;

public record CreateProductVariantCommand(
    long ProductId,
    string? Title = null,
    string? Sku = null,
    string? Barcode = null,
    decimal? Price = null,
    decimal? CompareAtPrice = null,
    decimal? CostPrice = null,
    int StockQuantity = 0,
    decimal? Weight = null,
    bool IsActive = true,
    int SortOrder = 0) : IRequest<Result<ProductVariantResponse>>;

public record CreateProductVariantRequest(
    string? Title = null,
    string? Sku = null,
    string? Barcode = null,
    decimal? Price = null,
    decimal? CompareAtPrice = null,
    decimal? CostPrice = null,
    int StockQuantity = 0,
    decimal? Weight = null,
    bool IsActive = true,
    int SortOrder = 0);

public record ProductVariantResponse(
    long Id,
    long ProductId,
    string? Title,
    string? Sku,
    string? Barcode,
    decimal? Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    int StockQuantity,
    decimal? Weight,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
