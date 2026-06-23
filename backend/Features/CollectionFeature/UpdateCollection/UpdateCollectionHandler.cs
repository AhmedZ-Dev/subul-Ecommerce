using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.CollectionFeature.UpdateCollection;

public class UpdateCollectionHandler(AppDbContext context)
    : IRequestHandler<UpdateCollectionCommand, Result<UpdateCollectionResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "manual", "smart"
    };

    public async Task<Result<UpdateCollectionResponse>> Handle(
        UpdateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var collection = await context.Collections
            .Include(c => c.CollectionProducts)
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (collection is null)
            return Result<UpdateCollectionResponse>.Failure("Collection not found");

        var normalizedNameEn = command.NameEn.Trim();
        var normalizedType = command.CollectionType.Trim().ToLowerInvariant();

        if (!ValidTypes.Contains(normalizedType))
            return Result<UpdateCollectionResponse>.Failure("Invalid collection type");

        var nameExists = await context.Collections.AnyAsync(
            c => c.Id != command.Id && c.NameEn.ToLower() == normalizedNameEn.ToLower(),
            cancellationToken);

        if (nameExists)
            return Result<UpdateCollectionResponse>.Failure("Collection name already exists");

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, collection.Id, cancellationToken);
        var now = DateTime.Now;

        collection.NameEn = normalizedNameEn;
        collection.NameAr = command.NameAr?.Trim();
        collection.Slug = slug;
        collection.DescriptionEn = command.DescriptionEn;
        collection.DescriptionAr = command.DescriptionAr;
        collection.ImageUrl = command.ImageUrl;
        collection.BannerUrl = command.BannerUrl;
        collection.CollectionType = normalizedType;
        collection.IsActive = command.IsActive;
        collection.SortOrder = command.SortOrder;
        collection.MetaTitle = command.MetaTitle;
        collection.MetaDescription = command.MetaDescription;
        collection.UpdatedAt = now;

        // Clear existing product associations
        context.CollectionProducts.RemoveRange(collection.CollectionProducts);

        if (command.Products is not null && command.Products.Count > 0)
        {
            var inputProductIds = command.Products.Select(p => p.ProductId).Distinct().ToList();
            var existingProductIds = await context.Products
                .Where(p => inputProductIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (existingProductIds.Count != inputProductIds.Count)
                return Result<UpdateCollectionResponse>.Failure("One or more products do not exist");

            foreach (var item in command.Products)
            {
                collection.CollectionProducts.Add(new CollectionProduct
                {
                    ProductId = item.ProductId,
                    SortOrder = item.SortOrder,
                    CreatedAt = now
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        // Fetch products to map in response
        var productsResponse = new List<CollectionProductResponse>();
        if (collection.CollectionProducts.Count > 0)
        {
            var productIds = collection.CollectionProducts.Select(cp => cp.ProductId).ToList();
            var productsInfo = await context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            foreach (var cp in collection.CollectionProducts)
            {
                if (productsInfo.TryGetValue(cp.ProductId, out var prod))
                {
                    productsResponse.Add(new CollectionProductResponse(
                        prod.Id,
                        prod.NameEn,
                        prod.NameAr,
                        prod.Slug,
                        prod.Price,
                        cp.SortOrder));
                }
            }
        }

        var response = new UpdateCollectionResponse(
            collection.Id,
            collection.NameEn,
            collection.NameAr,
            collection.Slug,
            collection.DescriptionEn,
            collection.DescriptionAr,
            collection.ImageUrl,
            collection.BannerUrl,
            collection.CollectionType,
            collection.IsActive,
            collection.SortOrder,
            collection.MetaTitle,
            collection.MetaDescription,
            collection.CreatedAt,
            collection.UpdatedAt,
            productsResponse.OrderBy(p => p.SortOrder).ToList());

        return Result<UpdateCollectionResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueSlugAsync(
        string baseSlug,
        long excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.Collections.AnyAsync(
                   c => c.Slug == slug && c.Id != excludeId,
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
        return string.IsNullOrEmpty(slug) ? "collection" : slug;
    }
}
