using backend.Common.Results;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductImageFeature.GetByIdProductImage;

public class GetByIdProductImageHandler(AppDbContext context)
    : IRequestHandler<GetByIdProductImageQuery, Result<ProductImageResponse>>
{
    public async Task<Result<ProductImageResponse>> Handle(
        GetByIdProductImageQuery query,
        CancellationToken cancellationToken)
    {
        var image = await context.ProductImages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                pi => pi.Id == query.Id && pi.ProductId == query.ProductId,
                cancellationToken);

        if (image is null)
            return Result<ProductImageResponse>.Failure("Product image not found");

        return Result<ProductImageResponse>.Success(CreateProductImageHandler.MapResponse(image));
    }
}
