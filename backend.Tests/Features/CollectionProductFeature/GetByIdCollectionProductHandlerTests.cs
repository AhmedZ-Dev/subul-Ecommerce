using backend.Features.CollectionProductFeature.CreateCollectionProduct;
using backend.Features.CollectionProductFeature.GetByIdCollectionProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CollectionProductFeature;

[Collection("Database")]
public class GetByIdCollectionProductHandlerTests(DatabaseFixture fixture)
    : CollectionProductHandlerTestBase(fixture)
{
    [Fact]
    public async Task Handle_ExistingLink_ReturnsSuccess()
    {
        var (collectionId, linkId) = await SeedLinkAsync();
        await using var context = Fixture.CreateContext();
        var handler = new GetByIdCollectionProductHandler(context);

        var result = await handler.Handle(
            new GetByIdCollectionProductQuery(collectionId, linkId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(linkId, result.Value!.Id);
    }

    [Fact]
    public async Task Handle_NonExistentLink_ReturnsNotFound()
    {
        var (collectionId, _) = await SeedLinkAsync();
        await using var context = Fixture.CreateContext();
        var handler = new GetByIdCollectionProductHandler(context);

        var result = await handler.Handle(
            new GetByIdCollectionProductQuery(collectionId, 999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
