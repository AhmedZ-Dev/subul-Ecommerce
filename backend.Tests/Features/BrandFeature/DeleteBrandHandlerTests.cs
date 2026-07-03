using backend.Domain.Entities;
using backend.Features.BrandFeature.CreateBrand;
using backend.Features.BrandFeature.DeleteBrand;
using backend.Features.BrandFeature.UploadBrandLogo;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class DeleteBrandHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedBrandAsync(string name)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateBrandHandler(ctx);
        var result = await handler.Handle(new CreateBrandCommand(name), CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ExistingBrand_DeletesSuccessfully()
    {
        var id = await SeedBrandAsync("Delete Me Brand");
        await using var context = fixture.CreateContext();
        var storage = ProductImageTestHelpers.CreateStorageService();
        var handler = new DeleteBrandHandler(context, storage);

        var result = await handler.Handle(new DeleteBrandCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var storage = ProductImageTestHelpers.CreateStorageService();
        var handler = new DeleteBrandHandler(context, storage);

        var result = await handler.Handle(new DeleteBrandCommand(999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_BrandWithProducts_ReturnsBadRequest()
    {
        var brandId = await SeedBrandAsync("Brand With Products Delete");

        await using var context = fixture.CreateContext();

        context.Products.Add(new Product
        {
            NameEn = "Test Product For Brand Delete",
            Slug = $"test-product-brand-delete-{Guid.NewGuid():N}",
            BrandId = brandId,
            Price = 100,
            Currency = "IQD",
            Status = "active",
            StockQuantity = 5,
            IsFeatured = false,
            RequiresShipping = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();

        var storage = ProductImageTestHelpers.CreateStorageService();
        var handler = new DeleteBrandHandler(context, storage);
        var result = await handler.Handle(new DeleteBrandCommand(brandId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("products", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
