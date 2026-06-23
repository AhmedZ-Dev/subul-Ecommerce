using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductAttributeValueFeature.GetByIdProductAttributeValue;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductAttributeValueFeature;

[Collection("Database")]
public class GetByIdProductAttributeValueHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long ValueId)> SeedValueAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Get PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Get PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Processor", InputType: "text"),
            CancellationToken.None);

        var valueHandler = new CreateProductAttributeValueHandler(ctx);
        var value = await valueHandler.Handle(
            new CreateProductAttributeValueCommand(
                product.Value!.Id,
                attribute.Value!.Id,
                ValueText: "Core i5"),
            CancellationToken.None);

        return (product.Value.Id, value.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingValue_ReturnsSuccess()
    {
        var (productId, valueId) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new GetByIdProductAttributeValueQuery(productId, valueId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(valueId, result.Value!.Id);
        Assert.Equal("Processor", result.Value.Attribute.NameEn);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        var (productId, _) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new GetByIdProductAttributeValueQuery(productId, 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WrongProductId_ReturnsNotFound()
    {
        var (productId, valueId) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new GetByIdProductAttributeValueQuery(productId + 999, valueId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
