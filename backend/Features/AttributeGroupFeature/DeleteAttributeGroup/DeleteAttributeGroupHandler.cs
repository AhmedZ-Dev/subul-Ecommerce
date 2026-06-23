using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeGroupFeature.DeleteAttributeGroup;

public class DeleteAttributeGroupHandler(AppDbContext context)
    : IRequestHandler<DeleteAttributeGroupCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteAttributeGroupCommand command,
        CancellationToken cancellationToken)
    {
        var group = await context.AttributeGroups
            .Include(g => g.Attributes)
            .FirstOrDefaultAsync(g => g.Id == command.Id, cancellationToken);

        if (group is null)
            return Result<bool>.Failure("Attribute group not found");

        var attributeIds = group.Attributes.Select(a => a.Id).ToList();

        if (attributeIds.Count > 0)
        {
            var hasValues = await context.ProductAttributeValues
                .AnyAsync(pav => attributeIds.Contains(pav.AttributeId), cancellationToken);

            if (hasValues)
                return Result<bool>.Failure("Cannot delete attribute group because it is associated with product specifications");

            foreach (var attr in group.Attributes)
            {
                context.Attributes.Remove(attr);
            }
        }

        context.AttributeGroups.Remove(group);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
