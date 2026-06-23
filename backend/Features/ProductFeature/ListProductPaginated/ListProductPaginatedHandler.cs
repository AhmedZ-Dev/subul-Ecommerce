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

        if (query.BrandId is not null)
            productQuery = productQuery.Where(p => p.BrandId == query.BrandId);

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
                BrandIdValue = p.Brand != null ? (long?)p.Brand.Id : null,
                BrandName = p.Brand != null ? p.Brand.Name : null,
                BrandSlug = p.Brand != null ? p.Brand.Slug : null
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
                ? new ProductCategoryInfo(p.CategoryIdValue.Value, p.CategoryNameEn!, p.CategoryNameAr)
                : null,
            p.BrandIdValue is not null
                ? new ProductBrandInfo(p.BrandIdValue.Value, p.BrandName!, p.BrandSlug!)
                : null)).ToList();

        var response = new ListProductPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListProductPaginatedResponse>.Success(response);
    }
}
