using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductAttributeValueFeature;

[Collection("Database")]
public class CreateProductAttributeValueHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long AttributeId)> SeedProductAndTextAttributeAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: $"RAM {Guid.NewGuid():N}", InputType: "text"),
            CancellationToken.None);

        return (product.Value!.Id, attribute.Value!.Id);
    }

    [Fact]
    public async Task Handle_ValidTextValue_ReturnsSuccess()
    {
        var (productId, attributeId) = await SeedProductAndTextAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductAttributeValueHandler(context);
        var command = new CreateProductAttributeValueCommand(productId, attributeId, ValueText: "16GB");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(productId, result.Value!.ProductId);
        Assert.Equal(attributeId, result.Value.AttributeId);
        Assert.Equal("16GB", result.Value.ValueText);
        Assert.Equal("text", result.Value.Attribute.InputType);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        var (_, attributeId) = await SeedProductAndTextAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new CreateProductAttributeValueCommand(999999, attributeId, ValueText: "16GB"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_NonExistentAttribute_ReturnsNotFound()
    {
        var (productId, _) = await SeedProductAndTextAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new CreateProductAttributeValueCommand(productId, 999999, ValueText: "16GB"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateAttributeForProduct_ReturnsConflict()
    {
        var (productId, attributeId) = await SeedProductAndTextAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductAttributeValueHandler(context);

        await handler.Handle(
            new CreateProductAttributeValueCommand(productId, attributeId, ValueText: "16GB"),
            CancellationToken.None);

        var result = await handler.Handle(
            new CreateProductAttributeValueCommand(productId, attributeId, ValueText: "32GB"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_BooleanAttributeMissingValue_ReturnsBadRequest()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Bool Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Bool Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Touch Screen", InputType: "boolean"),
            CancellationToken.None);

        var handler = new CreateProductAttributeValueHandler(context);
        var result = await handler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, attribute.Value!.Id),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("boolean", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_NumberAttribute_StoresValueNumberOnly()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Num Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Num Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Display Size", InputType: "number"),
            CancellationToken.None);

        var handler = new CreateProductAttributeValueHandler(context);
        var result = await handler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, attribute.Value!.Id, ValueNumber: 16.5m),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(16.5m, result.Value!.ValueNumber);
        Assert.Null(result.Value.ValueText);
        Assert.Null(result.Value.ValueBoolean);
    }
}
