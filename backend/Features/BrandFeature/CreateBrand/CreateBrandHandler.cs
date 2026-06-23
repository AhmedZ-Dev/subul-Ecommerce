using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.BrandFeature.CreateBrand;

public class CreateBrandHandler(AppDbContext context)
    : IRequestHandler<CreateBrandCommand, Result<CreateBrandResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    public async Task<Result<CreateBrandResponse>> Handle(
        CreateBrandCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedName = command.Name.Trim();

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedName)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, null, cancellationToken);
        var now = DateTime.Now;

        var brand = new Brand
        {
            Name = normalizedName,
            Slug = slug,
            LogoUrl = command.LogoUrl,
            BannerUrl = command.BannerUrl,
            DescriptionEn = command.DescriptionEn,
            DescriptionAr = command.DescriptionAr,
            WebsiteUrl = command.WebsiteUrl?.Trim(),
            IsFeatured = command.IsFeatured,
            IsActive = command.IsActive,
            SortOrder = command.SortOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Brands.Add(brand);
        await context.SaveChangesAsync(cancellationToken);

        var response = new CreateBrandResponse(
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
            new BrandProductCountResponse(0));

        return Result<CreateBrandResponse>.Success(response);
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
