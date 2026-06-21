using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CategoryFeature.ListCategoryPaginated;

public class ListCategoryPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListCategoryPaginatedQuery, Result<ListCategoryPaginatedResponse>>
{
    public async Task<Result<ListCategoryPaginatedResponse>> Handle(
        ListCategoryPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var categoryQuery = context.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            categoryQuery = categoryQuery.Where(c =>
                c.NameEn.ToLower().Contains(search) ||
                (c.NameAr != null && c.NameAr.ToLower().Contains(search)) ||
                (c.DescriptionEn != null && c.DescriptionEn.ToLower().Contains(search)) ||
                (c.DescriptionAr != null && c.DescriptionAr.ToLower().Contains(search)) ||
                c.Slug.ToLower().Contains(search));
        }

        if (query.ParentId is not null)
            categoryQuery = categoryQuery.Where(c => c.ParentId == query.ParentId);

        if (query.IsActive is not null)
            categoryQuery = categoryQuery.Where(c => c.IsActive == query.IsActive);

        categoryQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("nameen", "asc") => categoryQuery.OrderBy(c => c.NameEn),
            ("nameen", "desc") => categoryQuery.OrderByDescending(c => c.NameEn),
            ("sortorder", "asc") => categoryQuery.OrderBy(c => c.SortOrder),
            ("sortorder", "desc") => categoryQuery.OrderByDescending(c => c.SortOrder),
            ("updatedat", "asc") => categoryQuery.OrderBy(c => c.UpdatedAt),
            ("updatedat", "desc") => categoryQuery.OrderByDescending(c => c.UpdatedAt),
            ("createdat", "asc") => categoryQuery.OrderBy(c => c.CreatedAt),
            _ => categoryQuery.OrderByDescending(c => c.CreatedAt)
        };

        var total = await categoryQuery.CountAsync(cancellationToken);

        var rawItems = await categoryQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(c => new
            {
                c.Id,
                c.ParentId,
                c.NameEn,
                c.NameAr,
                c.Slug,
                c.DescriptionEn,
                c.DescriptionAr,
                c.ImageUrl,
                c.BannerUrl,
                c.SortOrder,
                c.IsActive,
                c.SeoTitle,
                c.SeoDescription,
                c.CreatedAt,
                c.UpdatedAt,
                ParentCategoryId = c.Parent != null ? (long?)c.Parent.Id : null,
                ParentCategoryNameEn = c.Parent != null ? c.Parent.NameEn : null,
                ParentCategoryNameAr = c.Parent != null ? c.Parent.NameAr : null,
                ProductCount = context.Products.Count(p => p.CategoryId == c.Id),
                SubCategoryCount = context.Categories.Count(x => x.ParentId == c.Id)
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(c => new ListCategoryPaginatedItemResponse(
            c.Id,
            c.ParentId,
            c.NameEn,
            c.NameAr,
            c.Slug,
            c.DescriptionEn,
            c.DescriptionAr,
            c.ImageUrl,
            c.BannerUrl,
            c.SortOrder,
            c.IsActive,
            c.SeoTitle,
            c.SeoDescription,
            c.CreatedAt,
            c.UpdatedAt,
            c.ParentCategoryId is not null
                ? new CategoryParentInfo(c.ParentCategoryId.Value, c.ParentCategoryNameEn!, c.ParentCategoryNameAr)
                : null,
            new CategoryProductCountResponse(c.ProductCount, c.SubCategoryCount))).ToList();

        var response = new ListCategoryPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListCategoryPaginatedResponse>.Success(response);
    }
}
