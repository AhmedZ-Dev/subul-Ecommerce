using backend.Features.CartFeature.AddCartItem;
using backend.Features.CartFeature.GetCart;
using backend.Features.CartFeature.RemoveCartItem;
using backend.Features.CartFeature.UpdateCartItem;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CartFeature;

[Collection("Database")]
public class AddCartItemHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidGuestCart_AddsItem()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand("Cart Product", null, null, null, Price: 250, StockQuantity: 10),
            CancellationToken.None);

        var sessionId = Guid.NewGuid().ToString("N");
        var handler = new AddCartItemHandler(context);
        var result = await handler.Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 2),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(sessionId, result.Value!.SessionId);
        Assert.Single(result.Value.Cart.Items);
        Assert.Equal(2, result.Value.Cart.ItemCount);
        Assert.Equal(500m, result.Value.Cart.Subtotal);
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new AddCartItemHandler(context);
        var result = await handler.Handle(
            new AddCartItemCommand(Guid.NewGuid().ToString("N"), 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}

[Collection("Database")]
public class GetCartHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_NewSession_ReturnsEmptyCart()
    {
        await using var context = fixture.CreateContext();
        var sessionId = Guid.NewGuid().ToString("N");
        var handler = new GetCartHandler(context);
        var result = await handler.Handle(new GetCartQuery(sessionId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        Assert.Equal(0m, result.Value.Subtotal);
    }
}

[Collection("Database")]
public class UpdateCartItemHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidQuantity_UpdatesItem()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand("Update Cart Product", null, null, null, Price: 100, StockQuantity: 10),
            CancellationToken.None);

        var sessionId = Guid.NewGuid().ToString("N");
        var addHandler = new AddCartItemHandler(context);
        var added = await addHandler.Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 1),
            CancellationToken.None);

        var cartItemId = added.Value!.Cart.Items[0].Id;
        var handler = new UpdateCartItemHandler(context);
        var result = await handler.Handle(
            new UpdateCartItemCommand(cartItemId, sessionId, 3),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.Items[0].Quantity);
        Assert.Equal(300m, result.Value.Subtotal);
    }
}

[Collection("Database")]
public class RemoveCartItemHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidItem_RemovesFromCart()
    {
        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand("Remove Cart Product", null, null, null, Price: 100, StockQuantity: 10),
            CancellationToken.None);

        var sessionId = Guid.NewGuid().ToString("N");
        var addHandler = new AddCartItemHandler(context);
        var added = await addHandler.Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 1),
            CancellationToken.None);

        var cartItemId = added.Value!.Cart.Items[0].Id;
        var handler = new RemoveCartItemHandler(context);
        var result = await handler.Handle(
            new RemoveCartItemCommand(cartItemId, sessionId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var getHandler = new GetCartHandler(context);
        var cart = await getHandler.Handle(new GetCartQuery(sessionId), CancellationToken.None);
        Assert.Empty(cart.Value!.Items);
    }
}
