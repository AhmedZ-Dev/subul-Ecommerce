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

namespace backend.Features.CollectionFeature.CreateCollection;

public class CreateCollectionHandler(AppDbContext context)
    : IRequestHandler<CreateCollectionCommand, Result<CreateCollectionResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "manual", "smart"
    };

    public async Task<Result<CreateCollectionResponse>> Handle(
        CreateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedNameEn = command.NameEn.Trim();
        var normalizedType = command.CollectionType.Trim().ToLowerInvariant();

        if (!ValidTypes.Contains(normalizedType))
            return Result<CreateCollectionResponse>.Failure("Invalid collection type");

        var nameExists = await context.Collections.AnyAsync(
            c => c.NameEn.ToLower() == normalizedNameEn.ToLower(),
            cancellationToken);

        if (nameExists)
            return Result<CreateCollectionResponse>.Failure("Collection name already exists");

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(baseSlug, null, cancellationToken);
        var now = DateTime.Now;

        var collection = new Collection
        {
            NameEn = normalizedNameEn,
            NameAr = command.NameAr?.Trim(),
            Slug = slug,
            DescriptionEn = command.DescriptionEn,
            DescriptionAr = command.DescriptionAr,
            ImageUrl = command.ImageUrl,
            BannerUrl = command.BannerUrl,
            CollectionType = normalizedType,
            IsActive = command.IsActive,
            SortOrder = command.SortOrder,
            MetaTitle = command.MetaTitle,
            MetaDescription = command.MetaDescription,
            CreatedAt = now,
            UpdatedAt = now
        };

        if (command.Products is not null && command.Products.Count > 0)
        {
            var inputProductIds = command.Products.Select(p => p.ProductId).Distinct().ToList();
            var existingProductIds = await context.Products
                .Where(p => inputProductIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (existingProductIds.Count != inputProductIds.Count)
                return Result<CreateCollectionResponse>.Failure("One or more products do not exist");

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

        context.Collections.Add(collection);
        await context.SaveChangesAsync(cancellationToken);

        // Fetch products to map in response
        var productsResponse = new List<CollectionProductResponse>();
        if (collection.CollectionProducts.Count > 0)
        {
            var productIds = collection.CollectionProducts.Select(cp => cp.ProductId).ToList();
            var productsInfo = await context.Products
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

        var response = new CreateCollectionResponse(
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

        return Result<CreateCollectionResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueSlugAsync(
        string baseSlug,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.Collections.AnyAsync(
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
        return string.IsNullOrEmpty(slug) ? "collection" : slug;
    }
}
