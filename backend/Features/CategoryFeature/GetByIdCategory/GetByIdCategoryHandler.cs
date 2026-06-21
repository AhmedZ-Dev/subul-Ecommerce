using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CategoryFeature.GetByIdCategory;

public class GetByIdCategoryHandler(AppDbContext context)
    : IRequestHandler<GetByIdCategoryQuery, Result<GetByIdCategoryResponse>>
{
    public async Task<Result<GetByIdCategoryResponse>> Handle(
        GetByIdCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (category is null)
            return Result<GetByIdCategoryResponse>.Failure("Category not found");

        var productCount = await context.Products.CountAsync(
            p => p.CategoryId == category.Id,
            cancellationToken);

        var subCategoryCount = await context.Categories.CountAsync(
            c => c.ParentId == category.Id,
            cancellationToken);

        CategoryParentInfo? parentInfo = category.Parent is not null
            ? new CategoryParentInfo(category.Parent.Id, category.Parent.NameEn, category.Parent.NameAr)
            : null;

        var response = new GetByIdCategoryResponse(
            category.Id,
            category.ParentId,
            category.NameEn,
            category.NameAr,
            category.Slug,
            category.DescriptionEn,
            category.DescriptionAr,
            category.ImageUrl,
            category.BannerUrl,
            category.SortOrder,
            category.IsActive,
            category.SeoTitle,
            category.SeoDescription,
            category.CreatedAt,
            category.UpdatedAt,
            parentInfo,
            new CategoryProductCountResponse(productCount, subCategoryCount));

        return Result<GetByIdCategoryResponse>.Success(response);
    }
}
