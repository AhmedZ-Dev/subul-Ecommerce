using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductAttributeValueFeature.DeleteProductAttributeValue;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductAttributeValueFeature;

[Collection("Database")]
public class DeleteProductAttributeValueHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long ValueId)> SeedValueAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Delete PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Delete PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "RAM", InputType: "text"),
            CancellationToken.None);

        var valueHandler = new CreateProductAttributeValueHandler(ctx);
        var value = await valueHandler.Handle(
            new CreateProductAttributeValueCommand(
                product.Value!.Id,
                attribute.Value!.Id,
                ValueText: "16GB"),
            CancellationToken.None);

        return (product.Value.Id, value.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingValue_DeletesSuccessfully()
    {
        var (productId, valueId) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new DeleteProductAttributeValueCommand(productId, valueId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_NonExistentValue_ReturnsNotFound()
    {
        var (productId, _) = await SeedValueAsync();
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductAttributeValueHandler(context);

        var result = await handler.Handle(
            new DeleteProductAttributeValueCommand(productId, 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
