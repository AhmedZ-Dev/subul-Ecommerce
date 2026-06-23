using backend.Domain.Entities;
using backend.Features.CartFeature.AddCartItem;
using backend.Features.OrderFeature.CreateOrder;
using backend.Features.OrderFeature.TrackGuestOrder;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ShippingZoneFeature.CreateShippingZone;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.OrderFeature;

[Collection("Database")]
public class CreateOrderHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_GuestCheckout_CreatesOrderAndClearsCart()
    {
        await using var context = fixture.CreateContext();
        var sessionId = Guid.NewGuid().ToString("N");

        var zoneHandler = new CreateShippingZoneHandler(context);
        var zone = await zoneHandler.Handle(
            new CreateShippingZoneCommand(
                "Baghdad Zone",
                Governorates: ["Baghdad"],
                ShippingRates:
                [
                    new CreateShippingRateInput("Standard", null, "flat", 5000m, IsActive: true)
                ]),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand("Order Product", null, null, null, Price: 1000, StockQuantity: 5),
            CancellationToken.None);

        var addHandler = new AddCartItemHandler(context);
        await addHandler.Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 2),
            CancellationToken.None);

        var handler = new CreateOrderHandler(context);
        var result = await handler.Handle(
            new CreateOrderCommand(
                SessionId: sessionId,
                ShippingFirstName: "Ahmed",
                ShippingLastName: "Ali",
                ShippingPhone: "07701234567",
                ShippingAddress1: "Street 1",
                ShippingCity: "Baghdad",
                ShippingGovernorate: "Baghdad",
                ShippingZoneId: zone.Value!.Id,
                PaymentMethod: "cod"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.UserId);
        Assert.StartsWith("ORD-", result.Value.OrderNumber);
        Assert.Equal("pending", result.Value.Status);
        Assert.Equal(2000m, result.Value.Subtotal);
        Assert.Equal(5000m, result.Value.ShippingAmount);
        Assert.Equal(7000m, result.Value.Total);
        Assert.Single(result.Value.Items);

        var cartItems = context.CartItems.Count(ci => ci.Cart.SessionId == sessionId);
        Assert.Equal(0, cartItems);
    }

    [Fact]
    public async Task Handle_GuestWithoutShipping_ReturnsValidationError()
    {
        await using var context = fixture.CreateContext();
        var sessionId = Guid.NewGuid().ToString("N");
        var handler = new CreateOrderHandler(context);

        var result = await handler.Handle(
            new CreateOrderCommand(SessionId: sessionId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("guest checkout", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}

[Collection("Database")]
public class TrackGuestOrderHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidOrderNumberAndPhone_ReturnsOrder()
    {
        await using var context = fixture.CreateContext();
        var sessionId = Guid.NewGuid().ToString("N");

        var zoneHandler = new CreateShippingZoneHandler(context);
        var zone = await zoneHandler.Handle(
            new CreateShippingZoneCommand(
                "Track Zone",
                Governorates: ["Basra"],
                ShippingRates:
                [
                    new CreateShippingRateInput("Standard", null, "flat", 3000m, IsActive: true)
                ]),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand("Track Product", null, null, null, Price: 500, StockQuantity: 3),
            CancellationToken.None);

        var addHandler = new AddCartItemHandler(context);
        await addHandler.Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 1),
            CancellationToken.None);

        var createHandler = new CreateOrderHandler(context);
        var created = await createHandler.Handle(
            new CreateOrderCommand(
                SessionId: sessionId,
                ShippingFirstName: "Sara",
                ShippingPhone: "07709998888",
                ShippingAddress1: "Basra Road",
                ShippingCity: "Basra",
                ShippingGovernorate: "Basra",
                ShippingZoneId: zone.Value!.Id),
            CancellationToken.None);

        var handler = new TrackGuestOrderHandler(context);
        var result = await handler.Handle(
            new TrackGuestOrderQuery(created.Value!.OrderNumber, "0770-999-8888"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(created.Value.OrderNumber, result.Value!.OrderNumber);
        Assert.Equal("pending", result.Value.Status);
    }

    [Fact]
    public async Task Handle_WrongPhone_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new TrackGuestOrderHandler(context);
        var result = await handler.Handle(
            new TrackGuestOrderQuery("ORD-20260101-123456", "07700000000"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
