using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductFeature.UpdateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

[Collection("Database")]
public class UpdateProductHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedProductAsync(string nameEn, string? sku = null)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateProductHandler(ctx);
        var result = await handler.Handle(
            new CreateProductCommand(nameEn, null, null, null, Sku: sku, Price: 100),
            CancellationToken.None);
        return result.Value!.Id;
    }

    private static UpdateProductCommand BuildUpdateCommand(long id, string nameEn, string? sku = null) =>
        new(
            Id: id,
            NameEn: nameEn,
            NameAr: null,
            CategoryId: null,
            BrandId: null,
            Slug: null,
            Sku: sku,
            Barcode: null,
            DescriptionEn: null,
            DescriptionAr: null,
            ShortDescriptionEn: null,
            ShortDescriptionAr: null,
            Price: 200,
            CompareAtPrice: null,
            CostPrice: null,
            Currency: "IQD",
            StockQuantity: 10,
            LowStockThreshold: 2,
            MinOrderQuantity: 1,
            Weight: null,
            Status: "active",
            IsFeatured: true,
            RequiresShipping: true,
            WarrantyMonths: 12,
            WarrantyDescription: null,
            MetaTitle: null,
            MetaDescription: null);

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var id = await SeedProductAsync("Update Target Product");
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductHandler(context);
        var command = BuildUpdateCommand(id, "Updated Product Name");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Product Name", result.Value!.NameEn);
        Assert.Equal(200, result.Value.Price);
        Assert.True(result.Value.IsFeatured);
        Assert.Equal(10, result.Value.StockQuantity);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductHandler(context);
        var command = BuildUpdateCommand(999999, "Ghost Product");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateSku_ReturnsConflict()
    {
        var skuA = $"SKU-A-{Guid.NewGuid():N}";
        var skuB = $"SKU-B-{Guid.NewGuid():N}";
        var idA = await SeedProductAsync("Update SKU Product A", skuA);
        var idB = await SeedProductAsync("Update SKU Product B", skuB);

        await using var context = fixture.CreateContext();
        var handler = new UpdateProductHandler(context);
        var command = BuildUpdateCommand(idB, "Update SKU Product B", skuA);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SameSkuForSameProduct_DoesNotConflict()
    {
        var sku = $"SKU-SAME-{Guid.NewGuid():N}";
        var id = await SeedProductAsync("Same SKU No Conflict", sku);

        await using var context = fixture.CreateContext();
        var handler = new UpdateProductHandler(context);
        var command = BuildUpdateCommand(id, "Same SKU No Conflict Updated", sku);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_InvalidStatus_ReturnsBadRequest()
    {
        var id = await SeedProductAsync("Invalid Status Update");
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductHandler(context);
        var command = BuildUpdateCommand(id, "Invalid Status Update") with { Status = "unknown" };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("status", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_NonExistentCategoryId_ReturnsNotFound()
    {
        var id = await SeedProductAsync("Category FK Update Test");
        await using var context = fixture.CreateContext();
        var handler = new UpdateProductHandler(context);
        var command = BuildUpdateCommand(id, "Category FK Update Test") with { CategoryId = 888888 };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
