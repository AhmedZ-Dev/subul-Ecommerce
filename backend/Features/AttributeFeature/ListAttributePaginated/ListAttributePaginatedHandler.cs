using backend.Common.Results;
using backend.Features.AttributeFeature.CreateAttribute;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeFeature.ListAttributePaginated;

public class ListAttributePaginatedHandler(AppDbContext context)
    : IRequestHandler<ListAttributePaginatedQuery, Result<ListAttributePaginatedResponse>>
{
    public async Task<Result<ListAttributePaginatedResponse>> Handle(
        ListAttributePaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var groupExists = await context.AttributeGroups.AnyAsync(
            g => g.Id == query.GroupId,
            cancellationToken);

        if (!groupExists)
            return Result<ListAttributePaginatedResponse>.Failure("Attribute group not found");

        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var attributeQuery = context.Attributes
            .AsNoTracking()
            .Where(a => a.GroupId == query.GroupId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            attributeQuery = attributeQuery.Where(a =>
                a.NameEn.ToLower().Contains(search) ||
                (a.NameAr != null && a.NameAr.ToLower().Contains(search)) ||
                (a.Slug != null && a.Slug.ToLower().Contains(search)));
        }

        if (query.IsFilterable is not null)
            attributeQuery = attributeQuery.Where(a => a.IsFilterable == query.IsFilterable);

        attributeQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("nameen", "asc") => attributeQuery.OrderBy(a => a.NameEn),
            ("nameen", "desc") => attributeQuery.OrderByDescending(a => a.NameEn),
            ("slug", "asc") => attributeQuery.OrderBy(a => a.Slug),
            ("slug", "desc") => attributeQuery.OrderByDescending(a => a.Slug),
            ("inputtype", "asc") => attributeQuery.OrderBy(a => a.InputType),
            ("inputtype", "desc") => attributeQuery.OrderByDescending(a => a.InputType),
            ("sortorder", "desc") => attributeQuery.OrderByDescending(a => a.SortOrder),
            ("createdat", "asc") => attributeQuery.OrderBy(a => a.CreatedAt),
            ("createdat", "desc") => attributeQuery.OrderByDescending(a => a.CreatedAt),
            _ => attributeQuery.OrderBy(a => a.SortOrder)
        };

        var total = await attributeQuery.CountAsync(cancellationToken);

        var attributes = await attributeQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = attributes
            .Select(CreateAttributeHandler.MapResponse)
            .ToList();

        var response = new ListAttributePaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListAttributePaginatedResponse>.Success(response);
    }
}
