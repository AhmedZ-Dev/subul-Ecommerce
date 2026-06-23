using backend.Features.CollectionFeature.CreateCollection;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CollectionProductFeature;

[Collection("Database")]
public class CreateCollectionProductHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long CollectionId, long ProductId)> SeedCollectionAndProductAsync()
    {
        await using var ctx = fixture.CreateContext();

        var collectionHandler = new CreateCollectionHandler(ctx);
        var collection = await collectionHandler.Handle(
            new CreateCollectionCommand($"Collection {Guid.NewGuid():N}", CollectionType: "manual"),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Product {Guid.NewGuid():N}", null, null, null, Price: 150),
            CancellationToken.None);

        return (collection.Value!.Id, product.Value!.Id);
    }

    [Fact]
    public async Task Handle_ValidLink_ReturnsSuccess()
    {
        var (collectionId, productId) = await SeedCollectionAndProductAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateCollectionProductHandler(context);

        var result = await handler.Handle(
            new CreateCollectionProductCommand(collectionId, productId, SortOrder: 1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(collectionId, result.Value!.CollectionId);
        Assert.Equal(productId, result.Value.ProductId);
        Assert.Equal(1, result.Value.SortOrder);
    }

    [Fact]
    public async Task Handle_NonExistentCollection_ReturnsNotFound()
    {
        var (_, productId) = await SeedCollectionAndProductAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateCollectionProductHandler(context);

        var result = await handler.Handle(
            new CreateCollectionProductCommand(999999999, productId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Collection not found", result.Error!);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        var (collectionId, _) = await SeedCollectionAndProductAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateCollectionProductHandler(context);

        var result = await handler.Handle(
            new CreateCollectionProductCommand(collectionId, 999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Product not found", result.Error!);
    }

    [Fact]
    public async Task Handle_DuplicateLink_ReturnsConflict()
    {
        var (collectionId, productId) = await SeedCollectionAndProductAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateCollectionProductHandler(context);

        await handler.Handle(
            new CreateCollectionProductCommand(collectionId, productId),
            CancellationToken.None);

        var result = await handler.Handle(
            new CreateCollectionProductCommand(collectionId, productId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
