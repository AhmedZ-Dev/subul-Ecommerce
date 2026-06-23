using backend.Domain.Entities;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Features.PaymentMethodFeature.DeletePaymentMethod;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class DeletePaymentMethodHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingId_DeletesPaymentMethod()
    {
        await using var ctx = fixture.CreateContext();
        var createHandler = new CreatePaymentMethodHandler(ctx);
        var created = await createHandler.Handle(
            new CreatePaymentMethodCommand($"delete_{Guid.NewGuid():N}", Type: "offline"),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new DeletePaymentMethodHandler(context);

        var result = await handler.Handle(
            new DeletePaymentMethodCommand(created.Value!.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        var exists = await context.PaymentMethods.AnyAsync(pm => pm.Id == created.Value.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new DeletePaymentMethodHandler(context);

        var result = await handler.Handle(
            new DeletePaymentMethodCommand(999999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WithPaymentTransactions_ReturnsFailure()
    {
        await using var context = fixture.CreateContext();
        var createHandler = new CreatePaymentMethodHandler(context);
        var created = await createHandler.Handle(
            new CreatePaymentMethodCommand($"tx_{Guid.NewGuid():N}", Type: "offline"),
            CancellationToken.None);

        var product = new Product
        {
            NameEn = "PM Product " + Guid.NewGuid(),
            Slug = "pm-prod-" + Guid.NewGuid().ToString("N"),
            Status = "active",
            Price = 100m,
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Products.Add(product);

        var user = new User
        {
            Email = "pm_user_" + Guid.NewGuid() + "@test.com",
            StoreCredit = 0m,
            CreatedAt = DateTime.Now
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            OrderNumber = "ORD-" + Guid.NewGuid().ToString("N")[..10],
            Status = "pending",
            Subtotal = 100m,
            Total = 100m,
            ShippingAmount = 0m,
            PaymentMethod = created.Value!.Name,
            PaymentStatus = "pending",
            FulfillmentStatus = "unfulfilled",
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        context.PaymentTransactions.Add(new PaymentTransaction
        {
            OrderId = order.Id,
            PaymentMethodId = created.Value.Id,
            Amount = 100m,
            Currency = "IQD",
            Status = "pending",
            Type = "charge",
            CreatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();

        var handler = new DeletePaymentMethodHandler(context);
        var result = await handler.Handle(
            new DeletePaymentMethodCommand(created.Value.Id),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("payment transactions", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
