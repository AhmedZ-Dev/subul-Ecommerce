using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductFeature.GetProductFilterOptions;

public class GetProductFilterOptionsHandler(AppDbContext context)
    : IRequestHandler<GetProductFilterOptionsQuery, Result<ProductFilterOptionsResponse>>
{
    public async Task<Result<ProductFilterOptionsResponse>> Handle(
        GetProductFilterOptionsQuery query,
        CancellationToken cancellationToken)
    {
        var productQuery = context.Products
            .AsNoTracking()
            .Where(p => p.Status.ToLower() == "active");

        if (query.CategoryId is not null)
            productQuery = productQuery.Where(p => p.CategoryId == query.CategoryId);

        var brandCounts = await productQuery
            .Where(p => p.BrandId != null)
            .GroupBy(p => p.BrandId!.Value)
            .Select(g => new { BrandId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var brandIds = brandCounts.Select(b => b.BrandId).ToList();
        var brandEntities = brandIds.Count == 0
            ? []
            : await context.Brands
                .AsNoTracking()
                .Where(b => brandIds.Contains(b.Id))
                .ToListAsync(cancellationToken);

        var brands = brandCounts
            .Join(
                brandEntities,
                count => count.BrandId,
                brand => brand.Id,
                (count, brand) => new BrandFacet(brand.Id, brand.Name, brand.Slug, count.Count))
            .OrderBy(b => b.Name)
            .ToList();

        var priceStats = await productQuery
            .Select(p => (decimal?)p.Price)
            .ToListAsync(cancellationToken);

        var priceRange = priceStats.Count == 0
            ? new PriceRangeFacet(0, 0)
            : new PriceRangeFacet(priceStats.Min() ?? 0, priceStats.Max() ?? 0);

        var productIds = await productQuery.Select(p => p.Id).ToListAsync(cancellationToken);

        var attributeRows = productIds.Count == 0
            ? []
            : await context.ProductAttributeValues
                .AsNoTracking()
                .Where(av =>
                    productIds.Contains(av.ProductId) &&
                    av.Attribute.Group != null &&
                    av.Attribute.Group.IsFilterable)
                .Select(av => new
                {
                    GroupId = av.Attribute.GroupId!.Value,
                    av.Attribute.Group!.NameEn,
                    av.Attribute.Group.NameAr,
                    av.Attribute.Group.SortOrder,
                    av.ValueText,
                    av.ValueNumber,
                    av.ValueBoolean,
                })
                .ToListAsync(cancellationToken);

        var attributeGroups = attributeRows
            .Select(row => new
            {
                row.GroupId,
                row.NameEn,
                row.NameAr,
                row.SortOrder,
                Value = GetDisplayValue(row.ValueText, row.ValueNumber, row.ValueBoolean),
            })
            .Where(row => !string.IsNullOrWhiteSpace(row.Value))
            .GroupBy(row => new { row.GroupId, row.NameEn, row.NameAr, row.SortOrder })
            .OrderBy(g => g.Key.SortOrder)
            .ThenBy(g => g.Key.NameEn)
            .Select(g => new AttributeGroupFacet(
                g.Key.GroupId,
                g.Key.NameEn,
                g.Key.NameAr,
                g.GroupBy(x => x.Value!)
                    .Select(vg => new AttributeValueFacet(vg.Key, vg.Count()))
                    .OrderBy(v => v.Value)
                    .ToList()))
            .Where(g => g.Values.Count > 0)
            .ToList();

        return Result<ProductFilterOptionsResponse>.Success(
            new ProductFilterOptionsResponse(brands, priceRange, attributeGroups));
    }

    private static string? GetDisplayValue(string? valueText, decimal? valueNumber, bool? valueBoolean)
    {
        if (!string.IsNullOrWhiteSpace(valueText))
            return valueText.Trim();

        if (valueNumber is not null)
            return valueNumber.Value == decimal.Truncate(valueNumber.Value)
                ? ((long)valueNumber.Value).ToString()
                : valueNumber.Value.ToString("0.##");

        if (valueBoolean is not null)
            return valueBoolean.Value ? "true" : "false";

        return null;
    }
}
