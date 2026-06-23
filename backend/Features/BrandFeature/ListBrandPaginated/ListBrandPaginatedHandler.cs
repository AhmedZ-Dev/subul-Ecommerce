using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.ListBrandPaginated;

public class ListBrandPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListBrandPaginatedQuery, Result<ListBrandPaginatedResponse>>
{
    public async Task<Result<ListBrandPaginatedResponse>> Handle(
        ListBrandPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var brandQuery = context.Brands.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            brandQuery = brandQuery.Where(b =>
                b.Name.ToLower().Contains(search) ||
                (b.DescriptionEn != null && b.DescriptionEn.ToLower().Contains(search)) ||
                (b.DescriptionAr != null && b.DescriptionAr.ToLower().Contains(search)) ||
                b.Slug.ToLower().Contains(search));
        }

        if (query.IsActive is not null)
            brandQuery = brandQuery.Where(b => b.IsActive == query.IsActive);

        if (query.IsFeatured is not null)
            brandQuery = brandQuery.Where(b => b.IsFeatured == query.IsFeatured);

        brandQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("name", "asc") => brandQuery.OrderBy(b => b.Name),
            ("name", "desc") => brandQuery.OrderByDescending(b => b.Name),
            ("sortorder", "asc") => brandQuery.OrderBy(b => b.SortOrder),
            ("sortorder", "desc") => brandQuery.OrderByDescending(b => b.SortOrder),
            ("updatedat", "asc") => brandQuery.OrderBy(b => b.UpdatedAt),
            ("updatedat", "desc") => brandQuery.OrderByDescending(b => b.UpdatedAt),
            ("createdat", "asc") => brandQuery.OrderBy(b => b.CreatedAt),
            _ => brandQuery.OrderByDescending(b => b.CreatedAt)
        };

        var total = await brandQuery.CountAsync(cancellationToken);

        var rawItems = await brandQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(b => new
            {
                b.Id,
                b.Name,
                b.Slug,
                b.LogoUrl,
                b.BannerUrl,
                b.DescriptionEn,
                b.DescriptionAr,
                b.WebsiteUrl,
                b.IsFeatured,
                b.IsActive,
                b.SortOrder,
                b.CreatedAt,
                b.UpdatedAt,
                ProductCount = context.Products.Count(p => p.BrandId == b.Id)
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(b => new ListBrandPaginatedItemResponse(
            b.Id,
            b.Name,
            b.Slug,
            b.LogoUrl,
            b.BannerUrl,
            b.DescriptionEn,
            b.DescriptionAr,
            b.WebsiteUrl,
            b.IsFeatured,
            b.IsActive,
            b.SortOrder,
            b.CreatedAt,
            b.UpdatedAt,
            new BrandProductCountResponse(b.ProductCount))).ToList();

        var response = new ListBrandPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListBrandPaginatedResponse>.Success(response);
    }
}
