using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.CollectionFeature.UpdateCollection;

public record CollectionProductInput(long ProductId, int SortOrder = 0);

public record UpdateCollectionCommand(
    long Id,
    string NameEn,
    string? NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    string? ImageUrl,
    string? BannerUrl,
    string CollectionType,
    bool IsActive,
    int SortOrder,
    string? MetaTitle,
    string? MetaDescription,
    List<CollectionProductInput>? Products,
    string? Slug) : IRequest<Result<UpdateCollectionResponse>>;

public record UpdateCollectionRequest(
    string NameEn,
    string? NameAr,
    string? DescriptionEn,
    string? DescriptionAr,
    string? ImageUrl,
    string? BannerUrl,
    string CollectionType,
    bool IsActive,
    int SortOrder,
    string? MetaTitle,
    string? MetaDescription,
    List<CollectionProductInput>? Products,
    string? Slug);

public record UpdateCollectionResponse(
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
