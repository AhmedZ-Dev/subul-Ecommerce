using backend.Common.Results;
using MediatR;

namespace backend.Features.ProductFeature.GetProductFilterOptions;

public record GetProductFilterOptionsQuery(long? CategoryId = null)
    : IRequest<Result<ProductFilterOptionsResponse>>;

public record ProductFilterOptionsResponse(
    List<BrandFacet> Brands,
    PriceRangeFacet PriceRange,
    List<AttributeGroupFacet> AttributeGroups);

public record BrandFacet(long Id, string Name, string Slug, int Count);

public record PriceRangeFacet(decimal Min, decimal Max);

public record AttributeGroupFacet(
    long Id,
    string NameEn,
    string? NameAr,
    List<AttributeValueFacet> Values);

public record AttributeValueFacet(string Value, int Count);
