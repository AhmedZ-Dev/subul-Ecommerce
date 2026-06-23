using backend.Features.CollectionFeature.CreateCollection;
using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CollectionProductFeature;

public abstract class CollectionProductHandlerTestBase(DatabaseFixture fixture)
{
    protected DatabaseFixture Fixture { get; } = fixture;

    protected async Task<(long CollectionId, long LinkId)> SeedLinkAsync()
    {
        await using var ctx = Fixture.CreateContext();

        var collection = await new CreateCollectionHandler(ctx).Handle(
            new CreateCollectionCommand($"Col {Guid.NewGuid():N}", CollectionType: "manual"),
            CancellationToken.None);

        var product = await new CreateProductHandler(ctx).Handle(
            new CreateProductCommand($"Prod {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var link = await new CreateCollectionProductHandler(ctx).Handle(
            new CreateCollectionProductCommand(collection.Value!.Id, product.Value!.Id, SortOrder: 0),
            CancellationToken.None);

        return (collection.Value.Id, link.Value!.Id);
    }
}
