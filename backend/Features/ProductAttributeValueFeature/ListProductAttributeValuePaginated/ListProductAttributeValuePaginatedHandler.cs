using backend.Common.Results;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductAttributeValueFeature.ListProductAttributeValuePaginated;

public class ListProductAttributeValuePaginatedHandler(AppDbContext context)
    : IRequestHandler<ListProductAttributeValuePaginatedQuery, Result<ListProductAttributeValuePaginatedResponse>>
{
    public async Task<Result<ListProductAttributeValuePaginatedResponse>> Handle(
        ListProductAttributeValuePaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(
            p => p.Id == query.ProductId,
            cancellationToken);

        if (!productExists)
            return Result<ListProductAttributeValuePaginatedResponse>.Failure("Product not found");

        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var valueQuery = context.ProductAttributeValues
            .AsNoTracking()
            .Include(pav => pav.Attribute)
            .Where(pav => pav.ProductId == query.ProductId);

        if (query.AttributeId is not null)
            valueQuery = valueQuery.Where(pav => pav.AttributeId == query.AttributeId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            valueQuery = valueQuery.Where(pav =>
                (pav.ValueText != null && pav.ValueText.ToLower().Contains(search)) ||
                pav.Attribute.NameEn.ToLower().Contains(search) ||
                (pav.Attribute.NameAr != null && pav.Attribute.NameAr.ToLower().Contains(search)));
        }

        valueQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("attributeid", "asc") => valueQuery.OrderBy(pav => pav.AttributeId),
            ("attributeid", "desc") => valueQuery.OrderByDescending(pav => pav.AttributeId),
            ("valuetext", "asc") => valueQuery.OrderBy(pav => pav.ValueText),
            ("valuetext", "desc") => valueQuery.OrderByDescending(pav => pav.ValueText),
            ("createdat", "asc") => valueQuery.OrderBy(pav => pav.CreatedAt),
            _ => valueQuery.OrderByDescending(pav => pav.CreatedAt)
        };

        var total = await valueQuery.CountAsync(cancellationToken);

        var values = await valueQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = values
            .Select(pav => CreateProductAttributeValueHandler.MapResponse(pav, pav.Attribute))
            .ToList();

        var response = new ListProductAttributeValuePaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListProductAttributeValuePaginatedResponse>.Success(response);
    }
}
