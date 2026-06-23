using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductFeature.GetByIdProduct;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

[Collection("Database")]
public class GetByIdProductHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedProductAsync(string nameEn, decimal price = 100)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateProductHandler(ctx);
        var result = await handler.Handle(
            new CreateProductCommand(nameEn, null, null, null, Price: price),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsProductWithCorrectData()
    {
        var id = await SeedProductAsync("GetById Target Product", 2500);
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductHandler(context);

        var result = await handler.Handle(new GetByIdProductQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("GetById Target Product", result.Value.NameEn);
        Assert.Equal(2500, result.Value.Price);
        Assert.Null(result.Value.Category);
        Assert.Null(result.Value.Brand);
        Assert.Empty(result.Value.Variants);
        Assert.Empty(result.Value.AttributeValues);
    }

    [Fact]
    public async Task Handle_ProductWithVariants_ReturnsVariantsOrderedBySortOrder()
    {
        await using var context = fixture.CreateContext();
        var createProductHandler = new CreateProductHandler(context);
        var product = await createProductHandler.Handle(
            new CreateProductCommand($"Variant Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var createVariantHandler = new CreateProductVariantHandler(context);
        await createVariantHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "32GB / 1TB", SortOrder: 2, Price: 2100000m),
            CancellationToken.None);
        await createVariantHandler.Handle(
            new CreateProductVariantCommand(product.Value.Id, Title: "16GB / 512GB", SortOrder: 1, Price: 1625000m),
            CancellationToken.None);

        var handler = new GetByIdProductHandler(context);
        var result = await handler.Handle(new GetByIdProductQuery(product.Value.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Variants.Count);
        Assert.Equal("16GB / 512GB", result.Value.Variants[0].Title);
        Assert.Equal(1625000m, result.Value.Variants[0].Price);
        Assert.Equal("32GB / 1TB", result.Value.Variants[1].Title);
    }

    [Fact]
    public async Task Handle_ProductWithAttributeValues_ReturnsValuesOrderedByAttributeSortOrder()
    {
        await using var context = fixture.CreateContext();
        var createProductHandler = new CreateProductHandler(context);
        var product = await createProductHandler.Handle(
            new CreateProductCommand($"Spec Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Spec Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var createAttributeHandler = new CreateAttributeHandler(context);
        var gpu = await createAttributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "GPU", InputType: "text", SortOrder: 2),
            CancellationToken.None);
        var ram = await createAttributeHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "RAM", InputType: "text", SortOrder: 1),
            CancellationToken.None);

        var createValueHandler = new CreateProductAttributeValueHandler(context);
        await createValueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, gpu.Value!.Id, ValueText: "RTX 5050"),
            CancellationToken.None);
        await createValueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value.Id, ram.Value!.Id, ValueText: "16GB"),
            CancellationToken.None);

        var handler = new GetByIdProductHandler(context);
        var result = await handler.Handle(new GetByIdProductQuery(product.Value.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.AttributeValues.Count);
        Assert.Equal("RAM", result.Value.AttributeValues[0].Attribute.NameEn);
        Assert.Equal("16GB", result.Value.AttributeValues[0].ValueText);
        Assert.Equal("GPU", result.Value.AttributeValues[1].Attribute.NameEn);
        Assert.Equal("RTX 5050", result.Value.AttributeValues[1].ValueText);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductHandler(context);

        var result = await handler.Handle(new GetByIdProductQuery(999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
