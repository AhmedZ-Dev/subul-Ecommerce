using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.UpdateBrand;

public class UpdateBrandHandler(AppDbContext context)
    : IRequestHandler<UpdateBrandCommand, Result<UpdateBrandResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    public async Task<Result<UpdateBrandResponse>> Handle(
        UpdateBrandCommand command,
        CancellationToken cancellationToken)
    {
        var brand = await context.Brands
            .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (brand is null)
            return Result<UpdateBrandResponse>.Failure("Brand not found");

        var normalizedName = command.Name.Trim();

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedName)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, command.Id, cancellationToken);

        brand.Name = normalizedName;
        brand.Slug = slug;
        brand.DescriptionEn = command.DescriptionEn;
        brand.DescriptionAr = command.DescriptionAr;
        brand.WebsiteUrl = command.WebsiteUrl?.Trim();
        brand.IsFeatured = command.IsFeatured;
        brand.IsActive = command.IsActive;
        brand.SortOrder = command.SortOrder;
        brand.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        var productCount = await context.Products.CountAsync(
            p => p.BrandId == brand.Id,
            cancellationToken);

        var response = new UpdateBrandResponse(
            brand.Id,
            brand.Name,
            brand.Slug,
            brand.LogoUrl,
            brand.BannerUrl,
            brand.DescriptionEn,
            brand.DescriptionAr,
            brand.WebsiteUrl,
            brand.IsFeatured,
            brand.IsActive,
            brand.SortOrder,
            brand.CreatedAt,
            brand.UpdatedAt,
            new BrandProductCountResponse(productCount));

        return Result<UpdateBrandResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueSlugAsync(
        string baseSlug,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.Brands.AnyAsync(
                   b => b.Slug == slug && (excludeId == null || b.Id != excludeId),
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
        return string.IsNullOrEmpty(slug) ? "brand" : slug;
    }
}
