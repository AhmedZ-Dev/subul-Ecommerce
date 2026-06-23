using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductImageFeature.CreateProductImage;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Features.ProductImageFeature;

[Collection("Database")]
public class CreateProductImageHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedProductAsync()
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateProductHandler(ctx);
        var result = await handler.Handle(
            new CreateProductCommand($"Image Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ValidImage_ReturnsSuccess()
    {
        var productId = await SeedProductAsync();
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductImageHandler(context, storage);
        var file = ProductImageTestHelpers.CreatePngFormFile();

        var result = await handler.Handle(
            new CreateProductImageCommand(productId, file, null, "Front view", 0, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(productId, result.Value!.ProductId);
        Assert.Equal("Front view", result.Value.AltText);
        Assert.True(result.Value.IsPrimary);
        Assert.StartsWith($"/img/products/{productId}/", result.Value.ImageUrl);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductImageHandler(context, storage);
        var file = ProductImageTestHelpers.CreatePngFormFile();

        var result = await handler.Handle(
            new CreateProductImageCommand(999999, file, null, null, 0, false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InvalidVariant_ReturnsNotFound()
    {
        var productId = await SeedProductAsync();
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductImageHandler(context, storage);
        var file = ProductImageTestHelpers.CreatePngFormFile();

        var result = await handler.Handle(
            new CreateProductImageCommand(productId, file, 999999, null, 0, false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("variant not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InvalidExtension_ReturnsFailure()
    {
        var productId = await SeedProductAsync();
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductImageHandler(context, storage);
        var file = ProductImageTestHelpers.CreateInvalidFormFile();

        var result = await handler.Handle(
            new CreateProductImageCommand(productId, file, null, null, 0, false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid image file", result.Error!);
    }

    [Fact]
    public async Task Handle_IsPrimary_ClearsOtherPrimaryImages()
    {
        var productId = await SeedProductAsync();
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductImageHandler(context, storage);

        await handler.Handle(
            new CreateProductImageCommand(productId, ProductImageTestHelpers.CreatePngFormFile(), null, null, 0, true),
            CancellationToken.None);

        var second = await handler.Handle(
            new CreateProductImageCommand(productId, ProductImageTestHelpers.CreatePngFormFile("second.png"), null, null, 1, true),
            CancellationToken.None);

        Assert.True(second.IsSuccess);

        var primaryCount = await context.ProductImages
            .CountAsync(pi => pi.ProductId == productId && pi.IsPrimary);

        Assert.Equal(1, primaryCount);
    }

    [Fact]
    public async Task Handle_WithValidVariant_ReturnsSuccess()
    {
        var productId = await SeedProductAsync();
        await using var ctx = fixture.CreateContext();
        var variantHandler = new CreateProductVariantHandler(ctx);
        var variant = await variantHandler.Handle(
            new CreateProductVariantCommand(productId, "16GB", null, null, 1000, null, null, 5, null, true, 0),
            CancellationToken.None);

        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new CreateProductImageHandler(context, storage);

        var result = await handler.Handle(
            new CreateProductImageCommand(
                productId,
                ProductImageTestHelpers.CreatePngFormFile(),
                variant.Value!.Id,
                null,
                0,
                false),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(variant.Value.Id, result.Value!.VariantId);
    }
}
