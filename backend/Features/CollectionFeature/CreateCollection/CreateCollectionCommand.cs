using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionFeature.CreateCollection;

public record CollectionProductInput(long ProductId, int SortOrder = 0);

public record CreateCollectionCommand(
    string NameEn,
    string? NameAr = null,
    string? DescriptionEn = null,
    string? DescriptionAr = null,
    string? ImageUrl = null,
    string? BannerUrl = null,
    string CollectionType = "manual", // manual / smart
    bool IsActive = true,
    int SortOrder = 0,
    string? MetaTitle = null,
    string? MetaDescription = null,
    List<CollectionProductInput>? Products = null,
    string? Slug = null) : IRequest<Result<CreateCollectionResponse>>;

public record CreateCollectionResponse(
    long Id,
    string NameEn,
    string? NameAr,
    string Slug,
    string? DescriptionEn,
    string? DescriptionAr,
    string? ImageUrl,
    string? BannerUrl,
    string CollectionType,
    bool IsActive,
    int SortOrder,
    string? MetaTitle,
    string? MetaDescription,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<CollectionProductResponse> Products);

public record CollectionProductResponse(
    long ProductId,
    string NameEn,
    string? NameAr,
    string Slug,
    decimal Price,
    int SortOrder);
