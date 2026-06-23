using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductAttributeValueFeature.ListProductAttributeValuePaginated;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductAttributeValueFeature;

[Collection("Database")]
public class ListProductAttributeValueHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingProduct_ReturnsPaginatedValues()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"List PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"List PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        var ram = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "RAM", InputType: "text"),
            CancellationToken.None);
        var gpu = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "GPU", InputType: "text"),
            CancellationToken.None);

        var valueHandler = new CreateProductAttributeValueHandler(context);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, ram.Value!.Id, ValueText: "16GB"),
            CancellationToken.None);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value.Id, gpu.Value!.Id, ValueText: "RTX 5050"),
            CancellationToken.None);

        var handler = new ListProductAttributeValuePaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductAttributeValuePaginatedQuery(product.Value.Id, Page: 1, Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Total);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListProductAttributeValuePaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductAttributeValuePaginatedQuery(999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SearchFilter_ReturnsMatchingValues()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Search PAV Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Search PAV Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        var ram = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "RAM", InputType: "text"),
            CancellationToken.None);
        var gpu = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "GPU", InputType: "text"),
            CancellationToken.None);

        var valueHandler = new CreateProductAttributeValueHandler(context);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, ram.Value!.Id, ValueText: "16GB"),
            CancellationToken.None);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value.Id, gpu.Value!.Id, ValueText: "RTX 5050"),
            CancellationToken.None);

        var handler = new ListProductAttributeValuePaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductAttributeValuePaginatedQuery(product.Value.Id, Search: "RTX"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Total);
        Assert.Equal("RTX 5050", result.Value.Items[0].ValueText);
    }
}
