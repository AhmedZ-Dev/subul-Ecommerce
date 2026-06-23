using backend.Common.Results;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionProductFeature.UpdateCollectionProduct;

public class UpdateCollectionProductHandler(AppDbContext context)
    : IRequestHandler<UpdateCollectionProductCommand, Result<CollectionProductLinkResponse>>
{
    public async Task<Result<CollectionProductLinkResponse>> Handle(
        UpdateCollectionProductCommand command,
        CancellationToken cancellationToken)
    {
        var link = await context.CollectionProducts
            .Include(cp => cp.Product)
            .FirstOrDefaultAsync(
                cp => cp.Id == command.Id && cp.CollectionId == command.CollectionId,
                cancellationToken);

        if (link is null)
            return Result<CollectionProductLinkResponse>.Failure("Collection product not found");

        link.SortOrder = command.SortOrder;

        await context.SaveChangesAsync(cancellationToken);

        return Result<CollectionProductLinkResponse>.Success(
            CreateCollectionProductHandler.MapResponse(link, link.Product));
    }
}
