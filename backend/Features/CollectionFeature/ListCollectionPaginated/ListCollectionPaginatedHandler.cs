using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionFeature.ListCollectionPaginated;

public class ListCollectionPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListCollectionPaginatedQuery, Result<ListCollectionPaginatedResponse>>
{
    public async Task<Result<ListCollectionPaginatedResponse>> Handle(
        ListCollectionPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var collectionQuery = context.Collections.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            collectionQuery = collectionQuery.Where(c =>
                c.NameEn.ToLower().Contains(search) ||
                (c.NameAr != null && c.NameAr.ToLower().Contains(search)) ||
                (c.DescriptionEn != null && c.DescriptionEn.ToLower().Contains(search)) ||
                (c.DescriptionAr != null && c.DescriptionAr.ToLower().Contains(search)) ||
                c.Slug.ToLower().Contains(search));
        }

        if (query.IsActive is not null)
            collectionQuery = collectionQuery.Where(c => c.IsActive == query.IsActive);

        if (!string.IsNullOrWhiteSpace(query.CollectionType))
        {
            var type = query.CollectionType.Trim().ToLower();
            collectionQuery = collectionQuery.Where(c => c.CollectionType.ToLower() == type);
        }

        collectionQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("nameen", "asc") => collectionQuery.OrderBy(c => c.NameEn),
            ("nameen", "desc") => collectionQuery.OrderByDescending(c => c.NameEn),
            ("sortorder", "asc") => collectionQuery.OrderBy(c => c.SortOrder),
            ("sortorder", "desc") => collectionQuery.OrderByDescending(c => c.SortOrder),
            ("updatedat", "asc") => collectionQuery.OrderBy(c => c.UpdatedAt),
            ("updatedat", "desc") => collectionQuery.OrderByDescending(c => c.UpdatedAt),
            ("createdat", "asc") => collectionQuery.OrderBy(c => c.CreatedAt),
            _ => collectionQuery.OrderByDescending(c => c.CreatedAt)
        };

        var total = await collectionQuery.CountAsync(cancellationToken);

        var rawItems = await collectionQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(c => new
            {
                c.Id,
                c.NameEn,
                c.NameAr,
                c.Slug,
                c.DescriptionEn,
                c.DescriptionAr,
                c.ImageUrl,
                c.BannerUrl,
                c.CollectionType,
                c.IsActive,
                c.SortOrder,
                c.MetaTitle,
                c.MetaDescription,
                c.CreatedAt,
                c.UpdatedAt,
                ProductCount = context.CollectionProducts.Count(cp => cp.CollectionId == c.Id)
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(c => new ListCollectionPaginatedItemResponse(
            c.Id,
            c.NameEn,
            c.NameAr,
            c.Slug,
            c.DescriptionEn,
            c.DescriptionAr,
            c.ImageUrl,
            c.BannerUrl,
            c.CollectionType,
            c.IsActive,
            c.SortOrder,
            c.MetaTitle,
            c.MetaDescription,
            c.CreatedAt,
            c.UpdatedAt,
            c.ProductCount)).ToList();

        var response = new ListCollectionPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListCollectionPaginatedResponse>.Success(response);
    }
}
