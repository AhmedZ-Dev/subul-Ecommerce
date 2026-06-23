using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductVariantFeature;

[Collection("Database")]
public class CreateProductVariantHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedProductAsync(string nameEn)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateProductHandler(ctx);
        var result = await handler.Handle(
            new CreateProductCommand(nameEn, null, null, null, Price: 100),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var productId = await SeedProductAsync($"Variant Parent {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new CreateProductVariantHandler(context);
        var command = new CreateProductVariantCommand(
            productId,
            Title: "16GB / 512GB SSD",
            Sku: $"VAR-{Guid.NewGuid():N}",
            Price: 1625000m,
            StockQuantity: 10);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(productId, result.Value.ProductId);
        Assert.Equal("16GB / 512GB SSD", result.Value.Title);
        Assert.Equal(1625000m, result.Value.Price);
        Assert.True(result.Value.Id > 0);
    }

    [Fact]
    public async Task Handle_NonExistentProductId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateProductVariantHandler(context);
        var command = new CreateProductVariantCommand(999999, Title: "Orphan Variant");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateSku_ReturnsConflict()
    {
        var productId = await SeedProductAsync($"SKU Parent {Guid.NewGuid():N}");
        var sku = $"DUP-VAR-{Guid.NewGuid():N}";

        await using var context = fixture.CreateContext();
        var handler = new CreateProductVariantHandler(context);

        await handler.Handle(
            new CreateProductVariantCommand(productId, Title: "Variant A", Sku: sku),
            CancellationToken.None);

        var result = await handler.Handle(
            new CreateProductVariantCommand(productId, Title: "Variant B", Sku: sku),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InputWithWhitespace_TrimsFields()
    {
        var productId = await SeedProductAsync($"Trim Parent {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new CreateProductVariantHandler(context);
        var command = new CreateProductVariantCommand(
            productId,
            Title: "  32GB / 1TB SSD  ",
            Sku: "  SKU-TRIM-VAR  ",
            Barcode: "  BAR-123  ");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("32GB / 1TB SSD", result.Value!.Title);
        Assert.Equal("SKU-TRIM-VAR", result.Value.Sku);
        Assert.Equal("BAR-123", result.Value.Barcode);
    }
}
