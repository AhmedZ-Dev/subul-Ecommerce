using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Features.ProductImageFeature.ListProductImagePaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductImageFeature;

[Collection("Database")]
public class ListProductImageHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingProduct_ReturnsPaginatedImages()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"List Image Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var storage = ProductImageTestHelpers.CreateStorageService();
        var createHandler = new CreateProductImageHandler(ctx, storage);
        await createHandler.Handle(
            new CreateProductImageCommand(product.Value!.Id, ProductImageTestHelpers.CreatePngFormFile(), null, null, 0, true),
            CancellationToken.None);
        await createHandler.Handle(
            new CreateProductImageCommand(product.Value.Id, ProductImageTestHelpers.CreatePngFormFile("b.png"), null, null, 1, false),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListProductImagePaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductImagePaginatedQuery(product.Value.Id, Page: 1, Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Total);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListProductImagePaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductImagePaginatedQuery(999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
