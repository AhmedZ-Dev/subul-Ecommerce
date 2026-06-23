using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Features.ProductVariantFeature.GetByIdProductVariant;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductVariantFeature;

[Collection("Database")]
public class GetByIdProductVariantHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long VariantId)> SeedVariantAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"GetById Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var variantHandler = new CreateProductVariantHandler(ctx);
        var variant = await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "GetById Variant", Price: 500m),
            CancellationToken.None);

        return (product.Value.Id, variant.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingVariant_ReturnsSuccess()
    {
        var (productId, variantId) = await SeedVariantAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductVariantHandler(context);

        var result = await handler.Handle(
            new GetByIdProductVariantQuery(productId, variantId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(variantId, result.Value!.Id);
        Assert.Equal(productId, result.Value.ProductId);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        var (productId, _) = await SeedVariantAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductVariantHandler(context);

        var result = await handler.Handle(
            new GetByIdProductVariantQuery(productId, 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WrongProductId_ReturnsNotFound()
    {
        var (productId, variantId) = await SeedVariantAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdProductVariantHandler(context);

        var result = await handler.Handle(
            new GetByIdProductVariantQuery(productId + 999, variantId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
