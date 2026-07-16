using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Features.CollectionFeature.GetByIdCollection;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionFeature.GetBySlugCollection;

public class GetBySlugCollectionHandler(AppDbContext context)
    : IRequestHandler<GetBySlugCollectionQuery, Result<GetByIdCollectionResponse>>
{
    public async Task<Result<GetByIdCollectionResponse>> Handle(
        GetBySlugCollectionQuery query,
        CancellationToken cancellationToken)
    {
        var slug = query.Slug.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug))
            return Result<GetByIdCollectionResponse>.Failure("Collection not found");

        var collection = await context.Collections
            .AsNoTracking()
            .Include(c => c.CollectionProducts)
            .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug, cancellationToken);

        if (collection is null)
            return Result<GetByIdCollectionResponse>.Failure("Collection not found");

        var productsResponse = await GetByIdCollectionHandler.MapCollectionProductsAsync(
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
}
