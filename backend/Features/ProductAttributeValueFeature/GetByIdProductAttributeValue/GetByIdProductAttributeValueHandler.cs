using backend.Common.Results;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductAttributeValueFeature.GetByIdProductAttributeValue;

public class GetByIdProductAttributeValueHandler(AppDbContext context)
    : IRequestHandler<GetByIdProductAttributeValueQuery, Result<ProductAttributeValueResponse>>
{
    public async Task<Result<ProductAttributeValueResponse>> Handle(
        GetByIdProductAttributeValueQuery query,
        CancellationToken cancellationToken)
    {
        var value = await context.ProductAttributeValues
            .AsNoTracking()
            .Include(pav => pav.Attribute)
            .FirstOrDefaultAsync(
                pav => pav.Id == query.Id && pav.ProductId == query.ProductId,
                cancellationToken);

        if (value is null)
            return Result<ProductAttributeValueResponse>.Failure("Product attribute value not found");

        return Result<ProductAttributeValueResponse>.Success(
            CreateProductAttributeValueHandler.MapResponse(value, value.Attribute));
    }
}
