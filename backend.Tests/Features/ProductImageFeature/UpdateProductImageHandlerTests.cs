using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Features.ProductImageFeature.UpdateProductImage;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Features.ProductImageFeature;

[Collection("Database")]
public class UpdateProductImageHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long ImageId)> SeedTwoImagesAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Update Image Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var storage = ProductImageTestHelpers.CreateStorageService();
        var createHandler = new CreateProductImageHandler(ctx, storage);
        var first = await createHandler.Handle(
            new CreateProductImageCommand(product.Value!.Id, ProductImageTestHelpers.CreatePngFormFile(), null, "First", 0, true),
            CancellationToken.None);
        await createHandler.Handle(
            new CreateProductImageCommand(product.Value.Id, ProductImageTestHelpers.CreatePngFormFile("second.png"), null, "Second", 1, false),
            CancellationToken.None);

        return (product.Value.Id, first.Value!.Id);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var (productId, imageId) = await SeedTwoImagesAsync();
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductImageHandler(context);

        var result = await handler.Handle(
            new UpdateProductImageCommand(productId, imageId, null, "Updated alt", 2, false),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated alt", result.Value!.AltText);
        Assert.Equal(2, result.Value.SortOrder);
    }

    [Fact]
    public async Task Handle_SetPrimary_ClearsOtherPrimaryImages()
    {
        var (productId, imageId) = await SeedTwoImagesAsync();
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductImageHandler(context);

        var secondImageId = await context.ProductImages
            .Where(pi => pi.ProductId == productId && pi.Id != imageId)
            .Select(pi => pi.Id)
            .FirstAsync();

        var result = await handler.Handle(
            new UpdateProductImageCommand(productId, secondImageId, null, null, 1, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsPrimary);

        var primaryCount = await context.ProductImages
            .CountAsync(pi => pi.ProductId == productId && pi.IsPrimary);

        Assert.Equal(1, primaryCount);
    }

    [Fact]
    public async Task Handle_NonExistentImage_ReturnsNotFound()
    {
        var (productId, _) = await SeedTwoImagesAsync();
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductImageHandler(context);

        var result = await handler.Handle(
            new UpdateProductImageCommand(productId, 999999999, null, null, 0, false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
