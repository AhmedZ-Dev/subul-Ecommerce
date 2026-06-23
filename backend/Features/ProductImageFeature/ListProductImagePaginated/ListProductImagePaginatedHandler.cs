using backend.Common.Results;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductImageFeature.ListProductImagePaginated;

public class ListProductImagePaginatedHandler(AppDbContext context)
    : IRequestHandler<ListProductImagePaginatedQuery, Result<ListProductImagePaginatedResponse>>
{
    public async Task<Result<ListProductImagePaginatedResponse>> Handle(
        ListProductImagePaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(
            p => p.Id == query.ProductId,
            cancellationToken);

        if (!productExists)
            return Result<ListProductImagePaginatedResponse>.Failure("Product not found");

        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var imageQuery = context.ProductImages
            .AsNoTracking()
            .Where(pi => pi.ProductId == query.ProductId);

        if (query.VariantId is not null)
            imageQuery = imageQuery.Where(pi => pi.VariantId == query.VariantId);

        imageQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("createdat", "asc") => imageQuery.OrderBy(pi => pi.CreatedAt),
            ("createdat", "desc") => imageQuery.OrderByDescending(pi => pi.CreatedAt),
            ("sortorder", "desc") => imageQuery
                .OrderByDescending(pi => pi.SortOrder)
                .ThenByDescending(pi => pi.CreatedAt),
            _ => imageQuery
                .OrderBy(pi => pi.SortOrder)
                .ThenBy(pi => pi.CreatedAt)
        };

        var total = await imageQuery.CountAsync(cancellationToken);

        var images = await imageQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = images
            .Select(CreateProductImageHandler.MapResponse)
            .ToList();

        var response = new ListProductImagePaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListProductImagePaginatedResponse>.Success(response);
    }
}
