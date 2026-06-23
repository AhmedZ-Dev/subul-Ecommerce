using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;

public class CreateProductAttributeValueHandler(AppDbContext context)
    : IRequestHandler<CreateProductAttributeValueCommand, Result<ProductAttributeValueResponse>>
{
    public async Task<Result<ProductAttributeValueResponse>> Handle(
        CreateProductAttributeValueCommand command,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(
            p => p.Id == command.ProductId,
            cancellationToken);

        if (!productExists)
            return Result<ProductAttributeValueResponse>.Failure("Product not found");

        var attribute = await context.Attributes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == command.AttributeId, cancellationToken);

        if (attribute is null)
            return Result<ProductAttributeValueResponse>.Failure("Attribute not found");

        var valueExists = await context.ProductAttributeValues.AnyAsync(
            pav => pav.ProductId == command.ProductId && pav.AttributeId == command.AttributeId,
            cancellationToken);

        if (valueExists)
            return Result<ProductAttributeValueResponse>.Failure("Product attribute value already exists");

        var validation = ValidateAndNormalizeValues(
            attribute.InputType,
            command.ValueText,
            command.ValueNumber,
            command.ValueBoolean);

        if (!validation.IsSuccess)
            return Result<ProductAttributeValueResponse>.Failure(validation.Error!);

        var now = DateTime.Now;
        var value = new ProductAttributeValue
        {
            ProductId = command.ProductId,
            AttributeId = command.AttributeId,
            ValueText = validation.Value!.ValueText,
            ValueNumber = validation.Value.ValueNumber,
            ValueBoolean = validation.Value.ValueBoolean,
            CreatedAt = now
        };

        context.ProductAttributeValues.Add(value);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductAttributeValueResponse>.Success(MapResponse(value, attribute));
    }

    internal static Result<(string? ValueText, decimal? ValueNumber, bool? ValueBoolean)> ValidateAndNormalizeValues(
        string inputType,
        string? valueText,
        decimal? valueNumber,
        bool? valueBoolean)
    {
        var type = inputType.Trim().ToLowerInvariant();

        return type switch
        {
            "text" or "select" when string.IsNullOrWhiteSpace(valueText) =>
                Result<(string?, decimal?, bool?)>.Failure("Value text is required for this attribute type"),
            "text" or "select" =>
                Result<(string?, decimal?, bool?)>.Success((valueText.Trim(), null, null)),
            "number" when valueNumber is null =>
                Result<(string?, decimal?, bool?)>.Failure("Value number is required for this attribute type"),
            "number" =>
                Result<(string?, decimal?, bool?)>.Success((null, valueNumber, null)),
            "boolean" when valueBoolean is null =>
                Result<(string?, decimal?, bool?)>.Failure("Value boolean is required for this attribute type"),
            "boolean" =>
                Result<(string?, decimal?, bool?)>.Success((null, null, valueBoolean)),
            _ => Result<(string?, decimal?, bool?)>.Failure("Invalid attribute input type")
        };
    }

    internal static ProductAttributeValueResponse MapResponse(
        ProductAttributeValue value,
        AttributeEntity attribute) =>
        new(
            value.Id,
            value.ProductId,
            value.AttributeId,
            value.ValueText,
            value.ValueNumber,
            value.ValueBoolean,
            value.CreatedAt,
            new ProductAttributeValueAttributeInfo(
                attribute.NameEn,
                attribute.NameAr,
                attribute.Unit,
                attribute.InputType));
}
