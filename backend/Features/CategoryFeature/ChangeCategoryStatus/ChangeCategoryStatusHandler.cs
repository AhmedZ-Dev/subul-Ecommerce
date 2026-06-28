using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CategoryFeature.ChangeCategoryStatus;

public class ChangeCategoryStatusHandler(AppDbContext context)
    : IRequestHandler<ChangeCategoryStatusCommand, Result<ChangeCategoryStatusResponse>>
{
    public async Task<Result<ChangeCategoryStatusResponse>> Handle(
        ChangeCategoryStatusCommand command,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category is null)
            return Result<ChangeCategoryStatusResponse>.Failure("Category not found");

        category.IsActive = command.IsActive;
        category.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        var response = new ChangeCategoryStatusResponse(
            category.Id,
            category.IsActive,
            category.UpdatedAt);

        return Result<ChangeCategoryStatusResponse>.Success(response);
    }
}
