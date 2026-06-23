using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductAttributeValueFeature.UpdateProductAttributeValue;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductAttributeValueFeature;

[Collection("Database")]
public class UpdateProductAttributeValueHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long ValueId, long AttributeId)> SeedValueAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Update PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Update PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Storage", InputType: "text"),
            CancellationToken.None);

        var valueHandler = new CreateProductAttributeValueHandler(ctx);
        var value = await valueHandler.Handle(
            new CreateProductAttributeValueCommand(
                product.Value!.Id,
                attribute.Value!.Id,
                ValueText: "512GB SSD"),
            CancellationToken.None);

        return (product.Value.Id, value.Value!.Id, attribute.Value.Id);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var (productId, valueId, attributeId) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductAttributeValueHandler(context);
        var command = new UpdateProductAttributeValueCommand(
            productId,
            valueId,
            attributeId,
            ValueText: "1TB SSD",
            ValueNumber: null,
            ValueBoolean: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("1TB SSD", result.Value!.ValueText);
    }

    [Fact]
    public async Task Handle_NonExistentValue_ReturnsNotFound()
    {
        var (productId, _, attributeId) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductAttributeValueHandler(context);
        var command = new UpdateProductAttributeValueCommand(
            productId,
            999999,
            attributeId,
            ValueText: "Ghost",
            ValueNumber: null,
            ValueBoolean: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateAttributeForProduct_ReturnsConflict()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Dup PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Dup PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        var ram = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "RAM", InputType: "text"),
            CancellationToken.None);
        var gpu = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "GPU", InputType: "text"),
            CancellationToken.None);

        var createHandler = new CreateProductAttributeValueHandler(context);
        var ramValue = await createHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, ram.Value!.Id, ValueText: "16GB"),
            CancellationToken.None);
        await createHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value.Id, gpu.Value!.Id, ValueText: "RTX 5050"),
            CancellationToken.None);

        var updateHandler = new UpdateProductAttributeValueHandler(context);
        var result = await updateHandler.Handle(
            new UpdateProductAttributeValueCommand(
                product.Value.Id,
                ramValue.Value!.Id,
                gpu.Value.Id,
                ValueText: "16GB",
                ValueNumber: null,
                ValueBoolean: null),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
