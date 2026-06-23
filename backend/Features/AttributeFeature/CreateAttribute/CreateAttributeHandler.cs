using System.Text.RegularExpressions;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Features.AttributeFeature.CreateAttribute;

public class CreateAttributeHandler(AppDbContext context)
    : IRequestHandler<CreateAttributeCommand, Result<AttributeResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidInputTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "select", "boolean", "number"
    };

    public async Task<Result<AttributeResponse>> Handle(
        CreateAttributeCommand command,
        CancellationToken cancellationToken)
    {
        var groupExists = await context.AttributeGroups.AnyAsync(
            g => g.Id == command.GroupId,
            cancellationToken);

        if (!groupExists)
            return Result<AttributeResponse>.Failure("Attribute group not found");

        var normalizedInputType = command.InputType.Trim().ToLowerInvariant();
        if (!ValidInputTypes.Contains(normalizedInputType))
            return Result<AttributeResponse>.Failure("Invalid input type");

        var normalizedNameEn = command.NameEn.Trim();

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedNameEn)
            : NormalizeSlug(command.Slug);

        var slug = await EnsureUniqueSlugAsync(context, baseSlug, null, cancellationToken);
        var now = DateTime.Now;

        var attribute = new AttributeEntity
        {
            GroupId = command.GroupId,
            NameEn = normalizedNameEn,
            NameAr = command.NameAr?.Trim(),
            Slug = slug,
            Unit = command.Unit?.Trim(),
            InputType = normalizedInputType,
            IsFilterable = command.IsFilterable,
            SortOrder = command.SortOrder,
            CreatedAt = now
        };

        context.Attributes.Add(attribute);
        await context.SaveChangesAsync(cancellationToken);

        return Result<AttributeResponse>.Success(MapResponse(attribute));
    }

    internal static AttributeResponse MapResponse(AttributeEntity attribute) =>
        new(
            attribute.Id,
            attribute.GroupId,
            attribute.NameEn,
            attribute.NameAr,
            attribute.Slug ?? string.Empty,
            attribute.Unit,
            attribute.InputType,
            attribute.IsFilterable,
            attribute.SortOrder,
            attribute.CreatedAt);

    internal static async Task<string> EnsureUniqueSlugAsync(
        AppDbContext context,
        string baseSlug,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.Attributes.AnyAsync(
                   a => a.Slug == slug && (excludeId == null || a.Id != excludeId),
                   cancellationToken))
        {
            suffix++;
            slug = $"{baseSlug}-{suffix}";
        }

        return slug;
    }

    internal static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = InvalidSlugCharsRegex.Replace(slug, string.Empty);
        slug = WhitespaceRegex.Replace(slug, "-");
        slug = DuplicateHyphensRegex.Replace(slug, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "attribute" : slug;
    }
}
