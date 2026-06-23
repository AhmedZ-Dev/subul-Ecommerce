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
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Features.AttributeGroupFeature.UpdateAttributeGroup;

public class UpdateAttributeGroupHandler(AppDbContext context)
    : IRequestHandler<UpdateAttributeGroupCommand, Result<UpdateAttributeGroupResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidInputTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "select", "boolean", "number"
    };

    public async Task<Result<UpdateAttributeGroupResponse>> Handle(
        UpdateAttributeGroupCommand command,
        CancellationToken cancellationToken)
    {
        var group = await context.AttributeGroups
            .Include(g => g.Attributes)
            .FirstOrDefaultAsync(g => g.Id == command.Id, cancellationToken);

        if (group is null)
            return Result<UpdateAttributeGroupResponse>.Failure("Attribute group not found");

        var normalizedGroupNameEn = command.NameEn.Trim();

        var groupExists = await context.AttributeGroups.AnyAsync(
            g => g.Id != command.Id && g.NameEn.ToLower() == normalizedGroupNameEn.ToLower(),
            cancellationToken);

        if (groupExists)
            return Result<UpdateAttributeGroupResponse>.Failure("Attribute group name already exists");

        var baseGroupSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedGroupNameEn)
            : NormalizeSlug(command.Slug);

        var groupSlug = await EnsureUniqueGroupSlugAsync(baseGroupSlug, group.Id, cancellationToken);
        var now = DateTime.Now;

        group.NameEn = normalizedGroupNameEn;
        group.NameAr = command.NameAr?.Trim();
        group.Slug = groupSlug;
        group.SortOrder = command.SortOrder;
        group.IsFilterable = command.IsFilterable;

        // Sync attributes
        var inputIds = command.Attributes?
            .Where(a => a.Id.HasValue && a.Id.Value > 0)
            .Select(a => a.Id!.Value)
            .ToList() ?? new List<long>();

        var toDeleteAttributes = group.Attributes.Where(a => !inputIds.Contains(a.Id)).ToList();
        var toDeleteIds = toDeleteAttributes.Select(a => a.Id).ToList();

        if (toDeleteIds.Count > 0)
        {
            var hasValues = await context.ProductAttributeValues
                .AnyAsync(pav => toDeleteIds.Contains(pav.AttributeId), cancellationToken);

            if (hasValues)
                return Result<UpdateAttributeGroupResponse>.Failure("Cannot delete attributes because they are associated with product specifications");

            foreach (var attr in toDeleteAttributes)
            {
                context.Attributes.Remove(attr);
            }
        }

        if (command.Attributes is not null)
        {
            foreach (var attrInput in command.Attributes)
            {
                var inputTypeNormalized = attrInput.InputType.Trim().ToLowerInvariant();
                if (!ValidInputTypes.Contains(inputTypeNormalized))
                    return Result<UpdateAttributeGroupResponse>.Failure($"Invalid input type: '{attrInput.InputType}' for attribute '{attrInput.NameEn}'");

                var attrNameEn = attrInput.NameEn.Trim();
                var baseAttrSlug = string.IsNullOrWhiteSpace(attrInput.Slug)
                    ? NormalizeSlug(attrNameEn)
                    : NormalizeSlug(attrInput.Slug);

                if (attrInput.Id.HasValue && attrInput.Id.Value > 0)
                {
                    var existingAttr = group.Attributes.FirstOrDefault(a => a.Id == attrInput.Id.Value);
                    if (existingAttr is not null)
                    {
                        var attrSlug = await EnsureUniqueAttributeSlugAsync(baseAttrSlug, existingAttr.Id, cancellationToken);

                        existingAttr.NameEn = attrNameEn;
                        existingAttr.NameAr = attrInput.NameAr?.Trim();
                        existingAttr.Slug = attrSlug;
                        existingAttr.Unit = attrInput.Unit?.Trim();
                        existingAttr.InputType = inputTypeNormalized;
                        existingAttr.IsFilterable = attrInput.IsFilterable;
                        existingAttr.SortOrder = attrInput.SortOrder;
                    }
                }
                else
                {
                    var attrSlug = await EnsureUniqueAttributeSlugAsync(baseAttrSlug, null, cancellationToken);

                    group.Attributes.Add(new AttributeEntity
                    {
                        NameEn = attrNameEn,
                        NameAr = attrInput.NameAr?.Trim(),
                        Slug = attrSlug,
                        Unit = attrInput.Unit?.Trim(),
                        InputType = inputTypeNormalized,
                        IsFilterable = attrInput.IsFilterable,
                        SortOrder = attrInput.SortOrder,
                        CreatedAt = now
                    });
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        var mappedAttributes = group.Attributes.Select(a => new UpdateAttributeGroupAttributeResponse(
            a.Id,
            a.NameEn,
            a.NameAr,
            a.Slug ?? string.Empty,
            a.Unit,
            a.InputType,
            a.IsFilterable,
            a.SortOrder,
            a.CreatedAt)).OrderBy(a => a.SortOrder).ToList();

        var response = new UpdateAttributeGroupResponse(
            group.Id,
            group.NameEn,
            group.NameAr,
            group.Slug ?? string.Empty,
            group.SortOrder,
            group.IsFilterable,
            group.CreatedAt,
            mappedAttributes);

        return Result<UpdateAttributeGroupResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueGroupSlugAsync(
        string baseSlug,
        long excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.AttributeGroups.AnyAsync(
                   g => g.Slug == slug && g.Id != excludeId,
                   cancellationToken))
        {
            suffix++;
            slug = $"{baseSlug}-{suffix}";
        }

        return slug;
    }

    private async Task<string> EnsureUniqueAttributeSlugAsync(
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

    private static string NormalizeSlug(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = InvalidSlugCharsRegex.Replace(slug, string.Empty);
        slug = WhitespaceRegex.Replace(slug, "-");
        slug = DuplicateHyphensRegex.Replace(slug, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "attribute" : slug;
    }
}
