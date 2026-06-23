using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeFeature.DeleteAttribute;

public class DeleteAttributeHandler(AppDbContext context)
    : IRequestHandler<DeleteAttributeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteAttributeCommand command,
        CancellationToken cancellationToken)
    {
        var attribute = await context.Attributes
            .FirstOrDefaultAsync(
                a => a.Id == command.Id && a.GroupId == command.GroupId,
                cancellationToken);

        if (attribute is null)
            return Result<bool>.Failure("Attribute not found");

        var hasProductValues = await context.ProductAttributeValues
            .AnyAsync(pav => pav.AttributeId == command.Id, cancellationToken);

        if (hasProductValues)
            return Result<bool>.Failure("Cannot delete attribute because it is associated with product specifications");

        context.Attributes.Remove(attribute);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
