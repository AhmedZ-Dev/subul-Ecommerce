using System.Text.Json;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductFeature.ListProductPaginated;

public class ListProductPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListProductPaginatedQuery, Result<ListProductPaginatedResponse>>
{
    public async Task<Result<ListProductPaginatedResponse>> Handle(
        ListProductPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var productQuery = context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            productQuery = productQuery.Where(p =>
                p.NameEn.ToLower().Contains(search) ||
                (p.NameAr != null && p.NameAr.ToLower().Contains(search)) ||
                (p.Sku != null && p.Sku.ToLower().Contains(search)) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(search)) ||
                (p.ShortDescriptionEn != null && p.ShortDescriptionEn.ToLower().Contains(search)) ||
                (p.ShortDescriptionAr != null && p.ShortDescriptionAr.ToLower().Contains(search)) ||
                p.Slug.ToLower().Contains(search));
        }

        if (query.CategoryId is not null)
            productQuery = productQuery.Where(p => p.CategoryId == query.CategoryId);

        if (query.BrandIds is { Count: > 0 })
            productQuery = productQuery.Where(p => p.BrandId != null && query.BrandIds.Contains(p.BrandId.Value));
        else if (query.BrandId is not null)
            productQuery = productQuery.Where(p => p.BrandId == query.BrandId);

        if (query.MinPrice is not null)
            productQuery = productQuery.Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice is not null)
            productQuery = productQuery.Where(p => p.Price <= query.MaxPrice.Value);

        if (query.InStockOnly == true)
            productQuery = productQuery.Where(p => p.StockQuantity > 0);

        var attributeFilters = ParseAttributeFilters(query.Attrs);
        foreach (var (groupId, values) in attributeFilters)
        {
            productQuery = productQuery.Where(p => p.ProductAttributeValues.Any(av =>
                av.Attribute.GroupId == groupId &&
                (
                    (av.ValueText != null && values.Contains(av.ValueText)) ||
                    (av.ValueNumber != null && values.Contains(FormatNumberValue(av.ValueNumber.Value))) ||
                    (av.ValueBoolean == true && values.Contains("true")) ||
                    (av.ValueBoolean == false && values.Contains("false"))
                )));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim().ToLower();
            productQuery = productQuery.Where(p => p.Status.ToLower() == status);
        }

        if (query.IsFeatured is not null)
            productQuery = productQuery.Where(p => p.IsFeatured == query.IsFeatured);

        productQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("nameen", "asc") => productQuery.OrderBy(p => p.NameEn),
            ("nameen", "desc") => productQuery.OrderByDescending(p => p.NameEn),
            ("price", "asc") => productQuery.OrderBy(p => p.Price),
            ("price", "desc") => productQuery.OrderByDescending(p => p.Price),
            ("stockquantity", "asc") => productQuery.OrderBy(p => p.StockQuantity),
            ("stockquantity", "desc") => productQuery.OrderByDescending(p => p.StockQuantity),
            ("totalsold", "asc") => productQuery.OrderBy(p => p.TotalSold),
            ("totalsold", "desc") => productQuery.OrderByDescending(p => p.TotalSold),
            ("updatedat", "asc") => productQuery.OrderBy(p => p.UpdatedAt),
            ("updatedat", "desc") => productQuery.OrderByDescending(p => p.UpdatedAt),
            ("createdat", "asc") => productQuery.OrderBy(p => p.CreatedAt),
            _ => productQuery.OrderByDescending(p => p.CreatedAt)
        };

        var total = await productQuery.CountAsync(cancellationToken);

        var rawItems = await productQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(p => new
            {
                p.Id,
                p.CategoryId,
                p.BrandId,
                p.NameEn,
                p.NameAr,
                p.Slug,
                p.Sku,
                p.Barcode,
                p.ShortDescriptionEn,
                p.ShortDescriptionAr,
                p.Price,
                p.CompareAtPrice,
                p.Currency,
                p.StockQuantity,
                p.Status,
                p.IsFeatured,
                p.TotalSold,
                p.ViewsCount,
                p.CreatedAt,
                p.UpdatedAt,
                CategoryIdValue = p.Category != null ? (long?)p.Category.Id : null,
                CategoryNameEn = p.Category != null ? p.Category.NameEn : null,
                CategoryNameAr = p.Category != null ? p.Category.NameAr : null,
                CategorySlug = p.Category != null ? p.Category.Slug : null,
                BrandIdValue = p.Brand != null ? (long?)p.Brand.Id : null,
                BrandName = p.Brand != null ? p.Brand.Name : null,
                BrandSlug = p.Brand != null ? p.Brand.Slug : null,
                PrimaryImageUrl = context.ProductImages
                    .Where(pi => pi.ProductId == p.Id)
                    .OrderByDescending(pi => pi.IsPrimary)
                    .ThenBy(pi => pi.SortOrder)
                    .Select(pi => pi.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(p => new ListProductPaginatedItemResponse(
            p.Id,
            p.CategoryId,
            p.BrandId,
            p.NameEn,
            p.NameAr,
            p.Slug,
            p.Sku,
            p.Barcode,
            p.ShortDescriptionEn,
            p.ShortDescriptionAr,
            p.Price,
            p.CompareAtPrice,
            p.Currency,
            p.StockQuantity,
            p.Status,
            p.IsFeatured,
            p.TotalSold,
            p.ViewsCount,
            p.CreatedAt,
            p.UpdatedAt,
            p.CategoryIdValue is not null
                ? new ProductCategoryInfo(p.CategoryIdValue.Value, p.CategoryNameEn!, p.CategoryNameAr, p.CategorySlug!)
                : null,
            p.BrandIdValue is not null
                ? new ProductBrandInfo(p.BrandIdValue.Value, p.BrandName!, p.BrandSlug!)
                : null,
            p.PrimaryImageUrl)).ToList();

        var response = new ListProductPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListProductPaginatedResponse>.Success(response);
    }

    private static Dictionary<long, List<string>> ParseAttributeFilters(string? attrs)
    {
        if (string.IsNullOrWhiteSpace(attrs))
            return [];

        try
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(attrs);
            if (parsed is null)
                return [];

            return parsed
                .Where(kvp => long.TryParse(kvp.Key, out _))
                .ToDictionary(
                    kvp => long.Parse(kvp.Key),
                    kvp => kvp.Value
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Select(v => v.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList());
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static string FormatNumberValue(decimal value) =>
        value == decimal.Truncate(value)
            ? ((long)value).ToString()
            : value.ToString("0.##");
}
