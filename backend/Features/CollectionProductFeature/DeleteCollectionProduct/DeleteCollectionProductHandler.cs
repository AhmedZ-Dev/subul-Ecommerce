using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionProductFeature.DeleteCollectionProduct;

public class DeleteCollectionProductHandler(AppDbContext context)
    : IRequestHandler<DeleteCollectionProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCollectionProductCommand command,
        CancellationToken cancellationToken)
    {
        var link = await context.CollectionProducts
            .FirstOrDefaultAsync(
                cp => cp.Id == command.Id && cp.CollectionId == command.CollectionId,
                cancellationToken);

        if (link is null)
            return Result<bool>.Failure("Collection product not found");

        context.CollectionProducts.Remove(link);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
