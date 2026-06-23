using backend.Common.Results;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionProductFeature.ListCollectionProductPaginated;

public class ListCollectionProductPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListCollectionProductPaginatedQuery, Result<ListCollectionProductPaginatedResponse>>
{
    public async Task<Result<ListCollectionProductPaginatedResponse>> Handle(
        ListCollectionProductPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var collectionExists = await context.Collections.AnyAsync(
            c => c.Id == query.CollectionId,
            cancellationToken);

        if (!collectionExists)
            return Result<ListCollectionProductPaginatedResponse>.Failure("Collection not found");

        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var linkQuery = context.CollectionProducts
            .AsNoTracking()
            .Include(cp => cp.Product)
            .Where(cp => cp.CollectionId == query.CollectionId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            linkQuery = linkQuery.Where(cp =>
                cp.Product.NameEn.ToLower().Contains(search) ||
                (cp.Product.NameAr != null && cp.Product.NameAr.ToLower().Contains(search)) ||
                cp.Product.Slug.ToLower().Contains(search));
        }

        linkQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("createdat", "asc") => linkQuery.OrderBy(cp => cp.CreatedAt),
            ("createdat", "desc") => linkQuery.OrderByDescending(cp => cp.CreatedAt),
            ("name", "asc") => linkQuery.OrderBy(cp => cp.Product.NameEn),
            ("name", "desc") => linkQuery.OrderByDescending(cp => cp.Product.NameEn),
            ("sortorder", "desc") => linkQuery
                .OrderByDescending(cp => cp.SortOrder)
                .ThenByDescending(cp => cp.CreatedAt),
            _ => linkQuery
                .OrderBy(cp => cp.SortOrder)
                .ThenBy(cp => cp.CreatedAt)
        };

        var total = await linkQuery.CountAsync(cancellationToken);

        var links = await linkQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = links
            .Select(cp => CreateCollectionProductHandler.MapResponse(cp, cp.Product))
            .ToList();

        var response = new ListCollectionProductPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListCollectionProductPaginatedResponse>.Success(response);
    }
}
