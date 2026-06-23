using backend.Features.CartFeature.AddCartItem;
using backend.Features.OrderFeature.CreateOrder;
using backend.Features.OrderFeature.GetByIdOrder;
using backend.Features.OrderFeature.GetByIdOrderItem;
using backend.Features.OrderFeature.ListOrderItems;
using backend.Features.OrderFeature.ListOrderPaginated;
using backend.Features.OrderFeature.UpdateOrder;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ShippingZoneFeature.CreateShippingZone;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.OrderFeature;

[Collection("Database")]
public class OrderReadHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> CreateTestOrderAsync(AppDbContext context)
    {
        var sessionId = Guid.NewGuid().ToString("N");

        var zoneHandler = new CreateShippingZoneHandler(context);
        var zone = await zoneHandler.Handle(
            new CreateShippingZoneCommand(
                $"Zone {Guid.NewGuid():N}",
                Governorates: ["Baghdad"],
                ShippingRates: [new CreateShippingRateInput("Standard", null, "flat", 5000m, IsActive: true)]),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Product {Guid.NewGuid():N}", null, null, null, Price: 500, StockQuantity: 10),
            CancellationToken.None);

        await new AddCartItemHandler(context).Handle(
            new AddCartItemCommand(sessionId, product.Value!.Id, Quantity: 2),
            CancellationToken.None);

        var created = await new CreateOrderHandler(context).Handle(
            new CreateOrderCommand(
                SessionId: sessionId,
                ShippingFirstName: "Test",
                ShippingPhone: "07701110000",
                ShippingAddress1: "Street",
                ShippingCity: "Baghdad",
                ShippingGovernorate: "Baghdad",
                ShippingZoneId: zone.Value!.Id),
            CancellationToken.None);

        return created.Value!.Id;
    }

    [Fact]
    public async Task GetByIdOrder_ExistingOrder_ReturnsOrderWithItems()
    {
        await using var context = fixture.CreateContext();
        var orderId = await CreateTestOrderAsync(context);

        var result = await new GetByIdOrderHandler(context).Handle(
            new GetByIdOrderQuery(orderId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(2, result.Value.Items[0].Quantity);
        Assert.Equal(1000m, result.Value.Subtotal);
    }

    [Fact]
    public async Task ListOrderItems_ExistingOrder_ReturnsItems()
    {
        await using var context = fixture.CreateContext();
        var orderId = await CreateTestOrderAsync(context);

        var result = await new ListOrderItemsHandler(context).Handle(
            new ListOrderItemsQuery(orderId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.True(result.Value.Items[0].Id > 0);
    }

    [Fact]
    public async Task GetByIdOrderItem_ValidIds_ReturnsItem()
    {
        await using var context = fixture.CreateContext();
        var orderId = await CreateTestOrderAsync(context);

        var order = await new GetByIdOrderHandler(context).Handle(
            new GetByIdOrderQuery(orderId),
            CancellationToken.None);

        var itemId = order.Value!.Items[0].Id;
        var result = await new GetByIdOrderItemHandler(context).Handle(
            new GetByIdOrderItemQuery(orderId, itemId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(itemId, result.Value!.Id);
        Assert.Equal(500m, result.Value.UnitPrice);
    }

    [Fact]
    public async Task GetByIdOrderItem_WrongOrderId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var orderId = await CreateTestOrderAsync(context);

        var order = await new GetByIdOrderHandler(context).Handle(
            new GetByIdOrderQuery(orderId),
            CancellationToken.None);

        var itemId = order.Value!.Items[0].Id;
        var result = await new GetByIdOrderItemHandler(context).Handle(
            new GetByIdOrderItemQuery(999999, itemId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListOrderPaginated_ReturnsOrdersWithItems()
    {
        await using var context = fixture.CreateContext();
        await CreateTestOrderAsync(context);

        var result = await new ListOrderPaginatedHandler(context).Handle(
            new ListOrderPaginatedQuery(Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value!.Items);
        Assert.True(result.Value.Items[0].ItemCount > 0);
        Assert.NotEmpty(result.Value.Items[0].Items);
    }

    [Fact]
    public async Task UpdateOrder_ValidStatus_ReturnsUpdatedOrder()
    {
        await using var context = fixture.CreateContext();
        var orderId = await CreateTestOrderAsync(context);

        var result = await new UpdateOrderHandler(context).Handle(
            new UpdateOrderCommand(orderId, Status: "confirmed", TrackingNumber: "TRK-123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("confirmed", result.Value!.Status);
        Assert.Equal("TRK-123", result.Value.TrackingNumber);
        Assert.Single(result.Value.Items);
    }

    [Fact]
    public async Task UpdateOrder_InvalidStatus_ReturnsBadRequest()
    {
        await using var context = fixture.CreateContext();
        var orderId = await CreateTestOrderAsync(context);

        var result = await new UpdateOrderHandler(context).Handle(
            new UpdateOrderCommand(orderId, Status: "invalid_status"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
