using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeGroupFeature.GetByIdAttributeGroup;

public class GetByIdAttributeGroupHandler(AppDbContext context)
    : IRequestHandler<GetByIdAttributeGroupQuery, Result<GetByIdAttributeGroupResponse>>
{
    public async Task<Result<GetByIdAttributeGroupResponse>> Handle(
        GetByIdAttributeGroupQuery query,
        CancellationToken cancellationToken)
    {
        var group = await context.AttributeGroups
            .AsNoTracking()
            .Include(g => g.Attributes)
            .FirstOrDefaultAsync(g => g.Id == query.Id, cancellationToken);

        if (group is null)
            return Result<GetByIdAttributeGroupResponse>.Failure("Attribute group not found");

        var mappedAttributes = group.Attributes.Select(a => new GetByIdAttributeGroupAttributeResponse(
            a.Id,
            a.NameEn,
            a.NameAr,
            a.Slug ?? string.Empty,
            a.Unit,
            a.InputType,
            a.IsFilterable,
            a.SortOrder,
            a.CreatedAt)).OrderBy(a => a.SortOrder).ToList();

        var response = new GetByIdAttributeGroupResponse(
            group.Id,
            group.NameEn,
            group.NameAr,
            group.Slug ?? string.Empty,
            group.SortOrder,
            group.IsFilterable,
            group.CreatedAt,
            mappedAttributes);

        return Result<GetByIdAttributeGroupResponse>.Success(response);
    }
}
