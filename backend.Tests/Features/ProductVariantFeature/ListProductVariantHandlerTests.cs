using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Features.ProductVariantFeature.ListProductVariantPaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductVariantFeature;

[Collection("Database")]
public class ListProductVariantHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingProduct_ReturnsPaginatedVariants()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"List Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var variantHandler = new CreateProductVariantHandler(context);
        await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "Variant A", SortOrder: 1),
            CancellationToken.None);
        await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value.Id, Title: "Variant B", SortOrder: 2),
            CancellationToken.None);

        var handler = new ListProductVariantPaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductVariantPaginatedQuery(product.Value.Id, Page: 1, Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Total);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListProductVariantPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductVariantPaginatedQuery(999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SearchFilter_ReturnsMatchingVariants()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Search Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var variantHandler = new CreateProductVariantHandler(context);
        await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "16GB RAM", Sku: "RAM-16"),
            CancellationToken.None);
        await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value.Id, Title: "32GB RAM", Sku: "RAM-32"),
            CancellationToken.None);

        var handler = new ListProductVariantPaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductVariantPaginatedQuery(product.Value.Id, Search: "32GB"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Total);
        Assert.Contains("32GB", result.Value.Items[0].Title);
    }
}
