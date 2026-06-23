using backend.Common.Results;
using backend.Features.AttributeFeature.CreateAttribute;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AttributeFeature.UpdateAttribute;

public class UpdateAttributeHandler(AppDbContext context)
    : IRequestHandler<UpdateAttributeCommand, Result<AttributeResponse>>
{
    private static readonly HashSet<string> ValidInputTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "select", "boolean", "number"
    };

    public async Task<Result<AttributeResponse>> Handle(
        UpdateAttributeCommand command,
        CancellationToken cancellationToken)
    {
        var attribute = await context.Attributes
            .FirstOrDefaultAsync(
                a => a.Id == command.Id && a.GroupId == command.GroupId,
                cancellationToken);

        if (attribute is null)
            return Result<AttributeResponse>.Failure("Attribute not found");

        var normalizedInputType = command.InputType.Trim().ToLowerInvariant();
        if (!ValidInputTypes.Contains(normalizedInputType))
            return Result<AttributeResponse>.Failure("Invalid input type");

        var normalizedNameEn = command.NameEn.Trim();

        var baseSlug = string.IsNullOrWhiteSpace(command.Slug)
            ? CreateAttributeHandler.NormalizeSlug(normalizedNameEn)
            : CreateAttributeHandler.NormalizeSlug(command.Slug);

        var slug = await CreateAttributeHandler.EnsureUniqueSlugAsync(
            context,
            baseSlug,
            command.Id,
            cancellationToken);

        attribute.NameEn = normalizedNameEn;
        attribute.NameAr = command.NameAr?.Trim();
        attribute.Slug = slug;
        attribute.Unit = command.Unit?.Trim();
        attribute.InputType = normalizedInputType;
        attribute.IsFilterable = command.IsFilterable;
        attribute.SortOrder = command.SortOrder;

        await context.SaveChangesAsync(cancellationToken);

        return Result<AttributeResponse>.Success(CreateAttributeHandler.MapResponse(attribute));
    }
}
