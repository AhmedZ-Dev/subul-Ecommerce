using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductAttributeValueFeature.DeleteProductAttributeValue;

public class DeleteProductAttributeValueHandler(AppDbContext context)
    : IRequestHandler<DeleteProductAttributeValueCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProductAttributeValueCommand command,
        CancellationToken cancellationToken)
    {
        var value = await context.ProductAttributeValues
            .FirstOrDefaultAsync(
                pav => pav.Id == command.Id && pav.ProductId == command.ProductId,
                cancellationToken);

        if (value is null)
            return Result<bool>.Failure("Product attribute value not found");

        context.ProductAttributeValues.Remove(value);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
