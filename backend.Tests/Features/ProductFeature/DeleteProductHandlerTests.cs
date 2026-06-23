using backend.Domain.Entities;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductFeature.DeleteProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

[Collection("Database")]
public class DeleteProductHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ExistingProduct_DeletesSuccessfully()
    {
        var id = await SeedProductAsync("Delete Me Product");
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductHandler(context);

        var result = await handler.Handle(new DeleteProductCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductHandler(context);

        var result = await handler.Handle(new DeleteProductCommand(999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ProductWithVariants_ReturnsBadRequest()
    {
        var productId = await SeedProductAsync("Product With Variants Delete");

        await using var context = fixture.CreateContext();

        context.ProductVariants.Add(new ProductVariant
        {
            ProductId = productId,
            Title = "Default Variant",
            StockQuantity = 5,
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();

        var handler = new DeleteProductHandler(context);
        var result = await handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("variants", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
