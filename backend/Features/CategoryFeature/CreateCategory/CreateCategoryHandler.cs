using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CategoryFeature.CreateCategory;

public class CreateCategoryHandler(AppDbContext context)
    : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    public async Task<Result<CreateCategoryResponse>> Handle(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedNameEn = command.NameEn.Trim();

        var nameExists = await context.Categories.AnyAsync(
            c => c.NameEn.ToLower() == normalizedNameEn.ToLower(),
            cancellationToken);

        if (nameExists)
            return Result<CreateCategoryResponse>.Failure("Category name already exists");

        if (command.ParentId is not null)
        {
            var parentExists = await context.Categories.AnyAsync(
                c => c.Id == command.ParentId,
                cancellationToken);

            if (!parentExists)
                return Result<CreateCategoryResponse>.Failure("Parent category not found");
        }

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, null, cancellationToken);
        var now = DateTime.Now;

        var category = new Category
        {
            NameEn = normalizedNameEn,
            NameAr = command.NameAr?.Trim(),
            DescriptionEn = command.DescriptionEn,
            DescriptionAr = command.DescriptionAr,
            ParentId = command.ParentId,
            Slug = slug,
            ImageUrl = command.ImageUrl,
            BannerUrl = command.BannerUrl,
            SortOrder = command.SortOrder,
            IsActive = command.IsActive,
            SeoTitle = command.SeoTitle,
            SeoDescription = command.SeoDescription,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        var response = new CreateCategoryResponse(
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
            new CategoryProductCountResponse(0, 0));

        return Result<CreateCategoryResponse>.Success(response);
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
