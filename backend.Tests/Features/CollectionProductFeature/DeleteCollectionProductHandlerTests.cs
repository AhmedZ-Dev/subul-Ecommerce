using backend.Features.CollectionProductFeature.DeleteCollectionProduct;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Features.CollectionProductFeature;

[Collection("Database")]
public class DeleteCollectionProductHandlerTests(DatabaseFixture fixture)
    : CollectionProductHandlerTestBase(fixture)
{
    [Fact]
    public async Task Handle_ExistingLink_DeletesRecord()
    {
        var (collectionId, linkId) = await SeedLinkAsync();
        await using var context = Fixture.CreateContext();
        var handler = new DeleteCollectionProductHandler(context);

        var result = await handler.Handle(
            new DeleteCollectionProductCommand(collectionId, linkId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var exists = await context.CollectionProducts.AnyAsync(cp => cp.Id == linkId);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_NonExistentLink_ReturnsNotFound()
    {
        var (collectionId, _) = await SeedLinkAsync();
        await using var context = Fixture.CreateContext();
        var handler = new DeleteCollectionProductHandler(context);

        var result = await handler.Handle(
            new DeleteCollectionProductCommand(collectionId, 999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
