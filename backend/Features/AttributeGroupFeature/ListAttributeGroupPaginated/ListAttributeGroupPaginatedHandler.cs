using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeGroupFeature.ListAttributeGroupPaginated;

public class ListAttributeGroupPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListAttributeGroupPaginatedQuery, Result<ListAttributeGroupPaginatedResponse>>
{
    public async Task<Result<ListAttributeGroupPaginatedResponse>> Handle(
        ListAttributeGroupPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var groupsQuery = context.AttributeGroups.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            groupsQuery = groupsQuery.Where(g =>
                g.NameEn.ToLower().Contains(search) ||
                (g.NameAr != null && g.NameAr.ToLower().Contains(search)) ||
                (g.Slug != null && g.Slug.ToLower().Contains(search)));
        }

        if (query.IsFilterable is not null)
            groupsQuery = groupsQuery.Where(g => g.IsFilterable == query.IsFilterable);

        groupsQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("nameen", "asc") => groupsQuery.OrderBy(g => g.NameEn),
            ("nameen", "desc") => groupsQuery.OrderByDescending(g => g.NameEn),
            ("sortorder", "asc") => groupsQuery.OrderBy(g => g.SortOrder),
            ("sortorder", "desc") => groupsQuery.OrderByDescending(g => g.SortOrder),
            ("createdat", "asc") => groupsQuery.OrderBy(g => g.CreatedAt),
            _ => groupsQuery.OrderByDescending(g => g.CreatedAt)
        };

        var total = await groupsQuery.CountAsync(cancellationToken);

        var rawItems = await groupsQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(g => new
            {
                g.Id,
                g.NameEn,
                g.NameAr,
                g.Slug,
                g.SortOrder,
                g.IsFilterable,
                g.CreatedAt,
                AttributeCount = g.Attributes.Count
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(g => new ListAttributeGroupPaginatedItemResponse(
            g.Id,
            g.NameEn,
            g.NameAr,
            g.Slug ?? string.Empty,
            g.SortOrder,
            g.IsFilterable,
            g.CreatedAt,
            g.AttributeCount)).ToList();

        var response = new ListAttributeGroupPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListAttributeGroupPaginatedResponse>.Success(response);
    }
}
