using backend.Domain.Entities;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductVariantFeature.CreateProductVariant;
using backend.Features.ProductVariantFeature.DeleteProductVariant;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductVariantFeature;

[Collection("Database")]
public class DeleteProductVariantHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long VariantId)> SeedVariantAsync()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Delete Parent {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        var variantHandler = new CreateProductVariantHandler(ctx);
        var variant = await variantHandler.Handle(
            new CreateProductVariantCommand(product.Value!.Id, Title: "Delete Me"),
            CancellationToken.None);

        return (product.Value.Id, variant.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingVariant_DeletesSuccessfully()
    {
        var (productId, variantId) = await SeedVariantAsync();
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductVariantHandler(context);

        var result = await handler.Handle(
            new DeleteProductVariantCommand(productId, variantId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_NonExistentVariant_ReturnsNotFound()
    {
        var (productId, _) = await SeedVariantAsync();
        await using var context = fixture.CreateContext();
        var handler = new DeleteProductVariantHandler(context);

        var result = await handler.Handle(
            new DeleteProductVariantCommand(productId, 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_VariantInCart_ReturnsBadRequest()
    {
        var (productId, variantId) = await SeedVariantAsync();

        await using var context = fixture.CreateContext();
        var now = DateTime.Now;
        var cart = new Cart
        {
            SessionId = Guid.NewGuid().ToString("N"),
            ExpiresAt = now.AddDays(30),
            CreatedAt = now,
            UpdatedAt = now
        };
        context.Carts.Add(cart);
        await context.SaveChangesAsync();

        context.CartItems.Add(new CartItem
        {
            CartId = cart.Id,
            ProductId = productId,
            VariantId = variantId,
            Quantity = 1,
            CreatedAt = now,
            UpdatedAt = now
        });
        await context.SaveChangesAsync();

        var handler = new DeleteProductVariantHandler(context);
        var result = await handler.Handle(
            new DeleteProductVariantCommand(productId, variantId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("cart", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
