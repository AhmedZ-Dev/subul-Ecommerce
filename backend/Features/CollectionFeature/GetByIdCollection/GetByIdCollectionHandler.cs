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

        var productsResponse = new List<CollectionProductResponse>();
        if (collection.CollectionProducts.Count > 0)
        {
            var productIds = collection.CollectionProducts.Select(cp => cp.ProductId).ToList();
            var productsInfo = await context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            foreach (var cp in collection.CollectionProducts)
            {
                if (productsInfo.TryGetValue(cp.ProductId, out var prod))
                {
                    productsResponse.Add(new CollectionProductResponse(
                        prod.Id,
                        prod.NameEn,
                        prod.NameAr,
                        prod.Slug,
                        prod.Price,
                        cp.SortOrder));
                }
            }
        }

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
            productsResponse.OrderBy(p => p.SortOrder).ToList());

        return Result<GetByIdCollectionResponse>.Success(response);
    }
}
