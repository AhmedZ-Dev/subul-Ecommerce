using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CategoryFeature.DeleteCategory;

public class DeleteCategoryHandler(AppDbContext context)
    : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category is null)
            return Result<bool>.Failure("Category not found");

        var hasRelatedProducts = await context.Products
            .AnyAsync(p => p.CategoryId == command.Id, cancellationToken);

        if (hasRelatedProducts)
            return Result<bool>.Failure("Cannot delete category because it is associated with products");

        var hasSubCategories = await context.Categories
            .AnyAsync(c => c.ParentId == command.Id, cancellationToken);

        if (hasSubCategories)
            return Result<bool>.Failure("Cannot delete category because it has subcategories");

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
