using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionFeature.GetByIdCollection;

public record GetByIdCollectionQuery(long Id) : IRequest<Result<GetByIdCollectionResponse>>;

public record GetByIdCollectionResponse(
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
    string Currency,
    int SortOrder,
    string? PrimaryImageUrl);
