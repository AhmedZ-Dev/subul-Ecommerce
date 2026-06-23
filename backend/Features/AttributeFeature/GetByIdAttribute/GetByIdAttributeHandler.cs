using backend.Common.Results;
using backend.Features.AttributeFeature.CreateAttribute;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeFeature.GetByIdAttribute;

public class GetByIdAttributeHandler(AppDbContext context)
    : IRequestHandler<GetByIdAttributeQuery, Result<AttributeResponse>>
{
    public async Task<Result<AttributeResponse>> Handle(
        GetByIdAttributeQuery query,
        CancellationToken cancellationToken)
    {
        var attribute = await context.Attributes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.Id == query.Id && a.GroupId == query.GroupId,
                cancellationToken);

        if (attribute is null)
            return Result<AttributeResponse>.Failure("Attribute not found");

        return Result<AttributeResponse>.Success(CreateAttributeHandler.MapResponse(attribute));
    }
}
