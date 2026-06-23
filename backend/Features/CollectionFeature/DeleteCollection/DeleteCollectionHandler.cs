using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionFeature.DeleteCollection;

public class DeleteCollectionHandler(AppDbContext context)
    : IRequestHandler<DeleteCollectionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var collection = await context.Collections
            .Include(c => c.CollectionProducts)
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (collection is null)
            return Result<bool>.Failure("Collection not found");

        // Manually delete related products links
        context.CollectionProducts.RemoveRange(collection.CollectionProducts);

        context.Collections.Remove(collection);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
