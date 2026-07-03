using backend.Common.Results;
using backend.Common.Storage;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductImageFeature.DeleteProductImage;

public class DeleteProductImageHandler(AppDbContext context, IImageStorageService imageStorage)
    : IRequestHandler<DeleteProductImageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteProductImageCommand command,
        CancellationToken cancellationToken)
    {
        var image = await context.ProductImages
            .FirstOrDefaultAsync(
                pi => pi.Id == command.Id && pi.ProductId == command.ProductId,
                cancellationToken);

        if (image is null)
            return Result<bool>.Failure("Product image not found");

        var imageUrl = image.ImageUrl;

        context.ProductImages.Remove(image);
        await context.SaveChangesAsync(cancellationToken);

        if (imageUrl.StartsWith("/img/products/", StringComparison.OrdinalIgnoreCase))
            await imageStorage.DeleteByRelativePathAsync(imageUrl, cancellationToken);

        return Result<bool>.Success(true);
    }
}
