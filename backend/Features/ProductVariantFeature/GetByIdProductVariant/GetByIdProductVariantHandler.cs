using backend.Common.Results;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductVariantFeature.GetByIdProductVariant;

public class GetByIdProductVariantHandler(AppDbContext context)
    : IRequestHandler<GetByIdProductVariantQuery, Result<ProductVariantResponse>>
{
    public async Task<Result<ProductVariantResponse>> Handle(
        GetByIdProductVariantQuery query,
        CancellationToken cancellationToken)
    {
        var variant = await context.ProductVariants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.Id == query.Id && v.ProductId == query.ProductId,
                cancellationToken);

        if (variant is null)
            return Result<ProductVariantResponse>.Failure("Product variant not found");

        return Result<ProductVariantResponse>.Success(CreateProductVariantHandler.MapResponse(variant));
    }
}
