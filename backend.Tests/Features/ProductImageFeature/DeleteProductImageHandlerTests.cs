using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Features.ProductImageFeature.DeleteProductImage;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Features.ProductImageFeature;

[Collection("Database")]
public class DeleteProductImageHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingImage_ReturnsSuccessAndRemovesRecord()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Delete Image Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var storage = ProductImageTestHelpers.CreateStorageService();
        var createHandler = new CreateProductImageHandler(ctx, storage);
        var image = await createHandler.Handle(
            new CreateProductImageCommand(product.Value!.Id, ProductImageTestHelpers.CreatePngFormFile(), null, null, 0, false),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new DeleteProductImageHandler(context, storage);

        var result = await handler.Handle(
            new DeleteProductImageCommand(product.Value.Id, image.Value!.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var exists = await context.ProductImages.AnyAsync(pi => pi.Id == image.Value.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_NonExistentImage_ReturnsNotFound()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Delete Missing Image {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductImageHandler(context, storage);

        var result = await handler.Handle(
            new DeleteProductImageCommand(product.Value!.Id, 999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
