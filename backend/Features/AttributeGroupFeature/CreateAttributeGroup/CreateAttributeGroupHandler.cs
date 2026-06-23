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

namespace backend.Features.AttributeGroupFeature.CreateAttributeGroup;

public class CreateAttributeGroupHandler(AppDbContext context)
    : IRequestHandler<CreateAttributeGroupCommand, Result<CreateAttributeGroupResponse>>
{
    private static readonly Regex InvalidSlugCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex DuplicateHyphensRegex = new(@"-+", RegexOptions.Compiled);

    private static readonly HashSet<string> ValidInputTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "select", "boolean", "number"
    };

    public async Task<Result<CreateAttributeGroupResponse>> Handle(
        CreateAttributeGroupCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedGroupNameEn = command.NameEn.Trim();

        var groupExists = await context.AttributeGroups.AnyAsync(
            g => g.NameEn.ToLower() == normalizedGroupNameEn.ToLower(),
            cancellationToken);

        if (groupExists)
            return Result<CreateAttributeGroupResponse>.Failure("Attribute group name already exists");

        var baseGroupSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? NormalizeSlug(normalizedGroupNameEn)
            : NormalizeSlug(command.Slug);

        var groupSlug = await EnsureUniqueGroupSlugAsync(baseGroupSlug, null, cancellationToken);
        var now = DateTime.Now;

        var group = new AttributeGroup
        {
            NameEn = normalizedGroupNameEn,
            NameAr = command.NameAr?.Trim(),
            Slug = groupSlug,
            SortOrder = command.SortOrder,
            IsFilterable = command.IsFilterable,
            CreatedAt = now
        };

        if (command.Attributes is not null && command.Attributes.Count > 0)
        {
            // Validate input types for nested attributes
            foreach (var attrInput in command.Attributes)
            {
                var inputTypeNormalized = attrInput.InputType.Trim().ToLowerInvariant();
                if (!ValidInputTypes.Contains(inputTypeNormalized))
                    return Result<CreateAttributeGroupResponse>.Failure($"Invalid input type: '{attrInput.InputType}' for attribute '{attrInput.NameEn}'");
            }

            foreach (var attrInput in command.Attributes)
            {
                var attrNameEn = attrInput.NameEn.Trim();
                var baseAttrSlug = string.IsNullOrWhiteSpace(attrInput.Slug)
                    ? NormalizeSlug(attrNameEn)
                    : NormalizeSlug(attrInput.Slug);

                var attrSlug = await EnsureUniqueAttributeSlugAsync(baseAttrSlug, null, cancellationToken);

                group.Attributes.Add(new AttributeEntity
                {
                    NameEn = attrNameEn,
                    NameAr = attrInput.NameAr?.Trim(),
                    Slug = attrSlug,
                    Unit = attrInput.Unit?.Trim(),
                    InputType = attrInput.InputType.Trim().ToLowerInvariant(),
                    IsFilterable = attrInput.IsFilterable,
                    SortOrder = attrInput.SortOrder,
                    CreatedAt = now
                });
            }
        }

        context.AttributeGroups.Add(group);
        await context.SaveChangesAsync(cancellationToken);

        var mappedAttributes = group.Attributes.Select(a => new CreateAttributeGroupAttributeResponse(
            a.Id,
            a.NameEn,
            a.NameAr,
            a.Slug ?? string.Empty,
            a.Unit,
            a.InputType,
            a.IsFilterable,
            a.SortOrder,
            a.CreatedAt)).OrderBy(a => a.SortOrder).ToList();

        var response = new CreateAttributeGroupResponse(
            group.Id,
            group.NameEn,
            group.NameAr,
            group.Slug ?? string.Empty,
            group.SortOrder,
            group.IsFilterable,
            group.CreatedAt,
            mappedAttributes);

        return Result<CreateAttributeGroupResponse>.Success(response);
    }

    private async Task<string> EnsureUniqueGroupSlugAsync(
        string baseSlug,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var suffix = 1;

        while (await context.AttributeGroups.AnyAsync(
                   g => g.Slug == slug && (excludeId == null || g.Id != excludeId),
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
