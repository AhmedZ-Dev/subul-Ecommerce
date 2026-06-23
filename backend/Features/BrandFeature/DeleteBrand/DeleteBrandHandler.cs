using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.DeleteBrand;

public class DeleteBrandHandler(AppDbContext context)
    : IRequestHandler<DeleteBrandCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteBrandCommand command,
        CancellationToken cancellationToken)
    {
        var brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (brand is null)
            return Result<bool>.Failure("Brand not found");

        var hasRelatedProducts = await context.Products
            .AnyAsync(p => p.BrandId == command.Id, cancellationToken);

        if (hasRelatedProducts)
            return Result<bool>.Failure("Cannot delete brand because it is associated with products");

        context.Brands.Remove(brand);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
