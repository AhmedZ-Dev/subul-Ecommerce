using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CategoryFeature.UpdateCategory;

public class UpdateCategoryHandler(AppDbContext context)
    : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    public async Task<Result<UpdateCategoryResponse>> Handle(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category is null)
            return Result<UpdateCategoryResponse>.Failure("Category not found");

        var normalizedNameEn = command.NameEn.Trim();

        var nameExists = await context.Categories.AnyAsync(
            c => c.Id != command.Id && c.NameEn.ToLower() == normalizedNameEn.ToLower(),
            cancellationToken);

        if (nameExists)
            return Result<UpdateCategoryResponse>.Failure("Category name already exists");

        if (command.ParentId == command.Id)
            return Result<UpdateCategoryResponse>.Failure("Category cannot be its own parent");

        if (command.ParentId is not null)
        {
            var parentExists = await context.Categories.AnyAsync(
                c => c.Id == command.ParentId,
                cancellationToken);

            if (!parentExists)
                return Result<UpdateCategoryResponse>.Failure("Parent category not found");
        }

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, command.Id, cancellationToken);

        category.NameEn = normalizedNameEn;
        category.NameAr = command.NameAr?.Trim();
        category.DescriptionEn = command.DescriptionEn;
        category.DescriptionAr = command.DescriptionAr;
        category.ParentId = command.ParentId;
        category.Slug = slug;
        category.ImageUrl = command.ImageUrl;
        category.BannerUrl = command.BannerUrl;
        category.SortOrder = command.SortOrder;
        category.IsActive = command.IsActive;
        category.SeoTitle = command.SeoTitle;
        category.SeoDescription = command.SeoDescription;
        category.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        var productCount = await context.Products.CountAsync(
            p => p.CategoryId == category.Id,
            cancellationToken);

        var subCategoryCount = await context.Categories.CountAsync(
            c => c.ParentId == category.Id,
            cancellationToken);

        var response = new UpdateCategoryResponse(
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
            new CategoryProductCountResponse(productCount, subCategoryCount));

        return Result<UpdateCategoryResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueSlugAsync(
        string baseSlug,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.Categories.AnyAsync(
                   c => c.Slug == slug && (excludeId == null || c.Id != excludeId),
                   cancellationToken))
        {
            suffix++;
            slug = $"{baseSlug}-{suffix}";
        }

        return slug;
    }

    private static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = InvalidSlugCharsRegex.Replace(slug, string.Empty);
        slug = WhitespaceRegex.Replace(slug, "-");
        slug = DuplicateHyphensRegex.Replace(slug, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "category" : slug;
    }
}
