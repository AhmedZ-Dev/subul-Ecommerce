using backend.Features.CollectionProductFeature.UpdateCollectionProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CollectionProductFeature;

[Collection("Database")]
public class UpdateCollectionProductHandlerTests(DatabaseFixture fixture)
    : CollectionProductHandlerTestBase(fixture)
{
    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var (collectionId, linkId) = await SeedLinkAsync();
        await using var context = Fixture.CreateContext();
        var handler = new UpdateCollectionProductHandler(context);

        var result = await handler.Handle(
            new UpdateCollectionProductCommand(collectionId, linkId, SortOrder: 5),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.SortOrder);
    }

    [Fact]
    public async Task Handle_NonExistentLink_ReturnsNotFound()
    {
        var (collectionId, _) = await SeedLinkAsync();
        await using var context = Fixture.CreateContext();
        var handler = new UpdateCollectionProductHandler(context);

        var result = await handler.Handle(
            new UpdateCollectionProductCommand(collectionId, 999999999, SortOrder: 1),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
