using backend.Features.CollectionFeature.CreateCollection;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Features.CollectionProductFeature.ListCollectionProductPaginated;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CollectionProductFeature;

[Collection("Database")]
public class ListCollectionProductHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ReturnsPaginatedLinks()
    {
        await using var ctx = fixture.CreateContext();

        var collection = await new CreateCollectionHandler(ctx).Handle(
            new CreateCollectionCommand($"List Col {Guid.NewGuid():N}", CollectionType: "manual"),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(ctx);
        var product1 = await productHandler.Handle(
            new CreateProductCommand($"List P1 {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);
        var product2 = await productHandler.Handle(
            new CreateProductCommand($"List P2 {Guid.NewGuid():N}", null, null, null, Price: 200),
            CancellationToken.None);

        var createHandler = new CreateCollectionProductHandler(ctx);
        await createHandler.Handle(
            new CreateCollectionProductCommand(collection.Value!.Id, product1.Value!.Id, 0),
            CancellationToken.None);
        await createHandler.Handle(
            new CreateCollectionProductCommand(collection.Value.Id, product2.Value!.Id, 1),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListCollectionProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCollectionProductPaginatedQuery(collection.Value.Id, Page: 1, Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Total);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_NonExistentCollection_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListCollectionProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCollectionProductPaginatedQuery(999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Collection not found", result.Error!);
    }
}
