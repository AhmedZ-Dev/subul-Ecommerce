using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionFeature.GetByIdCollection;

public class GetByIdCollectionHandler(AppDbContext context)
    : IRequestHandler<GetByIdCollectionQuery, Result<GetByIdCollectionResponse>>
{
    public async Task<Result<GetByIdCollectionResponse>> Handle(
        GetByIdCollectionQuery query,
        CancellationToken cancellationToken)
    {
        var collection = await context.Collections
            .AsNoTracking()
            .Include(c => c.CollectionProducts)
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (collection is null)
            return Result<GetByIdCollectionResponse>.Failure("Collection not found");

        var productsResponse = await MapCollectionProductsAsync(
            context,
            collection.CollectionProducts.Select(cp => (cp.ProductId, cp.SortOrder)).ToList(),
            cancellationToken);

        var response = new GetByIdCollectionResponse(
            collection.Id,
            collection.NameEn,
            collection.NameAr,
            collection.Slug,
            collection.DescriptionEn,
            collection.DescriptionAr,
            collection.ImageUrl,
            collection.BannerUrl,
            collection.CollectionType,
            collection.IsActive,
            collection.SortOrder,
            collection.MetaTitle,
            collection.MetaDescription,
            collection.CreatedAt,
            collection.UpdatedAt,
            productsResponse);

        return Result<GetByIdCollectionResponse>.Success(response);
    }

    internal static async Task<List<CollectionProductResponse>> MapCollectionProductsAsync(
        AppDbContext context,
        List<(long ProductId, int SortOrder)> collectionProducts,
        CancellationToken cancellationToken)
    {
        if (collectionProducts.Count == 0)
            return new List<CollectionProductResponse>();

        var productIds = collectionProducts.Select(cp => cp.ProductId).ToList();
        var productsInfo = await context.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.NameEn,
                p.NameAr,
                p.Slug,
                p.Price,
                p.Currency,
                PrimaryImageUrl = context.ProductImages
                    .Where(pi => pi.ProductId == p.Id)
                    .OrderByDescending(pi => pi.IsPrimary)
                    .ThenBy(pi => pi.SortOrder)
                    .Select(pi => pi.ImageUrl)
                    .FirstOrDefault(),
            })
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var productsResponse = new List<CollectionProductResponse>();
        foreach (var (productId, sortOrder) in collectionProducts.OrderBy(cp => cp.SortOrder))
        {
            if (!productsInfo.TryGetValue(productId, out var prod))
                continue;

            productsResponse.Add(new CollectionProductResponse(
                prod.Id,
                prod.NameEn,
                prod.NameAr,
                prod.Slug,
                prod.Price,
                prod.Currency,
                sortOrder,
                prod.PrimaryImageUrl));
        }

        return productsResponse;
    }
}
