using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Features.ProductVariantFeature.UpdateProductVariant;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductVariantFeature;

[Collection("Database")]
public class UpdateProductVariantHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long VariantId)> SeedVariantAsync(string sku)
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Update Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var variantHandler = new CreateProductVariantHandler(ctx);
        var variant = await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "Original", Sku: sku, Price: 100m, StockQuantity: 5),
            CancellationToken.None);

        return (product.Value.Id, variant.Value!.Id);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var (productId, variantId) = await SeedVariantAsync($"UPD-{Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductVariantHandler(context);
        var command = new UpdateProductVariantCommand(
            productId,
            variantId,
            Title: "Updated Variant",
            Sku: $"UPD-{Guid.NewGuid():N}",
            Barcode: null,
            Price: 200m,
            CompareAtPrice: null,
            CostPrice: null,
            StockQuantity: 15,
            Weight: null,
            IsActive: true,
            SortOrder: 1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Variant", result.Value!.Title);
        Assert.Equal(200m, result.Value.Price);
        Assert.Equal(15, result.Value.StockQuantity);
    }

    [Fact]
    public async Task Handle_NonExistentVariant_ReturnsNotFound()
    {
        var (productId, _) = await SeedVariantAsync($"NF-{Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductVariantHandler(context);
        var command = new UpdateProductVariantCommand(
            productId,
            999999,
            Title: "Ghost",
            Sku: null,
            Barcode: null,
            Price: 100m,
            CompareAtPrice: null,
            CostPrice: null,
            StockQuantity: 0,
            Weight: null,
            IsActive: true,
            SortOrder: 0);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateSku_ReturnsConflict()
    {
        var skuA = $"SKU-A-{Guid.NewGuid():N}";
        var skuB = $"SKU-B-{Guid.NewGuid():N}";

        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Dup SKU Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var createHandler = new CreateProductVariantHandler(context);
        var variantA = await createHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "A", Sku: skuA),
            CancellationToken.None);
        await createHandler.Handle(
            new CreateProductVariantCommand(product.Value.Id, Title: "B", Sku: skuB),
            CancellationToken.None);

        var updateHandler = new UpdateProductVariantHandler(context);
        var result = await updateHandler.Handle(
            new UpdateProductVariantCommand(
                product.Value.Id,
                variantA.Value!.Id,
                Title: "A Updated",
                Sku: skuB,
                Barcode: null,
                Price: 100m,
                CompareAtPrice: null,
                CostPrice: null,
                StockQuantity: 0,
                Weight: null,
                IsActive: true,
                SortOrder: 0),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
