using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Features.ProductImageFeature.GetByIdProductImage;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductImageFeature;

[Collection("Database")]
public class GetByIdProductImageHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long ImageId)> SeedProductImageAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Get Image Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var storage = ProductImageTestHelpers.CreateStorageService();
        var imageHandler = new CreateProductImageHandler(ctx, storage);
        var image = await imageHandler.Handle(
            new CreateProductImageCommand(
                product.Value!.Id,
                ProductImageTestHelpers.CreatePngFormFile(),
                null,
                "Alt",
                0,
                false),
            CancellationToken.None);

        return (product.Value.Id, image.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingImage_ReturnsSuccess()
    {
        var (productId, imageId) = await SeedProductImageAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductImageHandler(context);

        var result = await handler.Handle(
            new GetByIdProductImageQuery(productId, imageId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(imageId, result.Value!.Id);
        Assert.Equal(productId, result.Value.ProductId);
    }

    [Fact]
    public async Task Handle_NonExistentImage_ReturnsNotFound()
    {
        var (productId, _) = await SeedProductImageAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductImageHandler(context);

        var result = await handler.Handle(
            new GetByIdProductImageQuery(productId, 999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
