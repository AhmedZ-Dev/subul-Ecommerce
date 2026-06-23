using backend.Common.Results;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductVariantFeature.ListProductVariantPaginated;

public class ListProductVariantPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListProductVariantPaginatedQuery, Result<ListProductVariantPaginatedResponse>>
{
    public async Task<Result<ListProductVariantPaginatedResponse>> Handle(
        ListProductVariantPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(
            p => p.Id == query.ProductId,
            cancellationToken);

        if (!productExists)
            return Result<ListProductVariantPaginatedResponse>.Failure("Product not found");

        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var variantQuery = context.ProductVariants
            .AsNoTracking()
            .Where(v => v.ProductId == query.ProductId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            variantQuery = variantQuery.Where(v =>
                (v.Title != null && v.Title.ToLower().Contains(search)) ||
                (v.Sku != null && v.Sku.ToLower().Contains(search)) ||
                (v.Barcode != null && v.Barcode.ToLower().Contains(search)));
        }

        if (query.IsActive is not null)
            variantQuery = variantQuery.Where(v => v.IsActive == query.IsActive);

        variantQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("title", "asc") => variantQuery.OrderBy(v => v.Title),
            ("title", "desc") => variantQuery.OrderByDescending(v => v.Title),
            ("sku", "asc") => variantQuery.OrderBy(v => v.Sku),
            ("sku", "desc") => variantQuery.OrderByDescending(v => v.Sku),
            ("price", "asc") => variantQuery.OrderBy(v => v.Price),
            ("price", "desc") => variantQuery.OrderByDescending(v => v.Price),
            ("stockquantity", "asc") => variantQuery.OrderBy(v => v.StockQuantity),
            ("stockquantity", "desc") => variantQuery.OrderByDescending(v => v.StockQuantity),
            ("sortorder", "desc") => variantQuery.OrderByDescending(v => v.SortOrder),
            ("createdat", "asc") => variantQuery.OrderBy(v => v.CreatedAt),
            ("createdat", "desc") => variantQuery.OrderByDescending(v => v.CreatedAt),
            ("updatedat", "asc") => variantQuery.OrderBy(v => v.UpdatedAt),
            ("updatedat", "desc") => variantQuery.OrderByDescending(v => v.UpdatedAt),
            _ => variantQuery.OrderBy(v => v.SortOrder)
        };

        var total = await variantQuery.CountAsync(cancellationToken);

        var variants = await variantQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = variants
            .Select(CreateProductVariantHandler.MapResponse)
            .ToList();

        var response = new ListProductVariantPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListProductVariantPaginatedResponse>.Success(response);
    }
}
