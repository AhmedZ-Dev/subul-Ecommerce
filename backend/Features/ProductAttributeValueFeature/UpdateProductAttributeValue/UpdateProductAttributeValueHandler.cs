using backend.Common.Results;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ProductAttributeValueFeature.UpdateProductAttributeValue;

public class UpdateProductAttributeValueHandler(AppDbContext context)
    : IRequestHandler<UpdateProductAttributeValueCommand, Result<ProductAttributeValueResponse>>
{
    public async Task<Result<ProductAttributeValueResponse>> Handle(
        UpdateProductAttributeValueCommand command,
        CancellationToken cancellationToken)
    {
        var value = await context.ProductAttributeValues
            .FirstOrDefaultAsync(
                pav => pav.Id == command.Id && pav.ProductId == command.ProductId,
                cancellationToken);

        if (value is null)
            return Result<ProductAttributeValueResponse>.Failure("Product attribute value not found");

        var attribute = await context.Attributes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == command.AttributeId, cancellationToken);

        if (attribute is null)
            return Result<ProductAttributeValueResponse>.Failure("Attribute not found");

        var duplicateExists = await context.ProductAttributeValues.AnyAsync(
            pav => pav.ProductId == command.ProductId &&
                   pav.AttributeId == command.AttributeId &&
                   pav.Id != command.Id,
            cancellationToken);

        if (duplicateExists)
            return Result<ProductAttributeValueResponse>.Failure("Product attribute value already exists");

        var validation = CreateProductAttributeValueHandler.ValidateAndNormalizeValues(
            attribute.InputType,
            command.ValueText,
            command.ValueNumber,
            command.ValueBoolean);

        if (!validation.IsSuccess)
            return Result<ProductAttributeValueResponse>.Failure(validation.Error!);

        value.AttributeId = command.AttributeId;
        value.ValueText = validation.Value!.ValueText;
        value.ValueNumber = validation.Value.ValueNumber;
        value.ValueBoolean = validation.Value.ValueBoolean;

        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductAttributeValueResponse>.Success(
            CreateProductAttributeValueHandler.MapResponse(value, attribute));
    }
}
