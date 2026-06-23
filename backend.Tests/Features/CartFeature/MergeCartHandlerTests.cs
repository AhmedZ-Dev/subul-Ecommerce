using backend.Domain.Entities;
using backend.Features.CartFeature.AddCartItem;
using backend.Features.CartFeature.MergeCart;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CartFeature;

[Collection("Database")]
public class MergeCartHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_GuestCartWithItems_MergesIntoUserCart()
    {
        await using var context = fixture.CreateContext();
        var sessionId = Guid.NewGuid().ToString("N");
        var now = DateTime.Now;

        var user = new User
        {
            Email = $"merge-{Guid.NewGuid():N}@test.com",
            IsActive = true,
            CreatedAt = now
        };
        context.Users.Add(user);

        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand("Merge Product", null, null, null, Price: 100, StockQuantity: 10),
            CancellationToken.None);

        var addHandler = new AddCartItemHandler(context);
        await addHandler.Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 2),
            CancellationToken.None);

        var handler = new MergeCartHandler(context);
        var result = await handler.Handle(new MergeCartCommand(sessionId, user.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value!.UserId);
        Assert.Single(result.Value.Items);
        Assert.Equal(2, result.Value.ItemCount);
    }
}
