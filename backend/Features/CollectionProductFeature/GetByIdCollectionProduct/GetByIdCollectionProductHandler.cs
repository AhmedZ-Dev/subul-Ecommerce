using backend.Common.Results;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionProductFeature.GetByIdCollectionProduct;

public class GetByIdCollectionProductHandler(AppDbContext context)
    : IRequestHandler<GetByIdCollectionProductQuery, Result<CollectionProductLinkResponse>>
{
    public async Task<Result<CollectionProductLinkResponse>> Handle(
        GetByIdCollectionProductQuery query,
        CancellationToken cancellationToken)
    {
        var link = await context.CollectionProducts
            .AsNoTracking()
            .Include(cp => cp.Product)
            .FirstOrDefaultAsync(
                cp => cp.Id == query.Id && cp.CollectionId == query.CollectionId,
                cancellationToken);

        if (link is null)
            return Result<CollectionProductLinkResponse>.Failure("Collection product not found");

        return Result<CollectionProductLinkResponse>.Success(
            CreateCollectionProductHandler.MapResponse(link, link.Product));
    }
}
