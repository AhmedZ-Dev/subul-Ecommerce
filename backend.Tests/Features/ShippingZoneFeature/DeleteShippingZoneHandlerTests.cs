using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.ShippingZoneFeature.DeleteShippingZone;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests.Features.ShippingZoneFeature;

[Collection("Database")]
public class DeleteShippingZoneHandlerTests(DatabaseFixture fixture)
{
    private async Task<ShippingZone> CreateZoneAsync(AppDbContext context, string name)
    {
        var zone = new ShippingZone
        {
            NameEn = name,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync();
        return zone;
    }

    [Fact]
    public async Task Handle_ExistingId_DeletesZoneAndRates()
    {
        await using var context = fixture.CreateContext();
        var zone = await CreateZoneAsync(context, "Zone for Delete " + Guid.NewGuid());
        var rate = new ShippingRate
        {
            NameEn = "Rate to Del",
            RateType = "flat",
            Price = 5000m,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        zone.ShippingRates.Add(rate);
        await context.SaveChangesAsync();

        var handler = new DeleteShippingZoneHandler(context);
        var command = new DeleteShippingZoneCommand(zone.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);

        var deletedZone = await context.ShippingZones.AnyAsync(z => z.Id == zone.Id);
        Assert.False(deletedZone);

        var deletedRate = await context.ShippingRates.AnyAsync(r => r.Id == rate.Id);
        Assert.False(deletedRate);
    }

    [Fact]
    public async Task Handle_ZoneWithOrders_ReturnsFailure()
    {
        await using var context = fixture.CreateContext();
        var zone = await CreateZoneAsync(context, "Zone with Orders " + Guid.NewGuid());

        // Create product
        var product = new Product
        {
            NameEn = "Test Product for Order " + Guid.NewGuid(),
            Slug = "test-prod-order-" + Guid.NewGuid().ToString("N"),
            Status = "active",
            Price = 100m,
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Create customer user
        var user = new User
        {
            Email = "customer_" + Guid.NewGuid() + "@test.com",
            StoreCredit = 0m,
            CreatedAt = DateTime.Now
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Create order
        var order = new Order
        {
            UserId = user.Id,
            ShippingZoneId = zone.Id,
            OrderNumber = "ORD-" + Guid.NewGuid().ToString("N").Substring(0, 10),
            Status = "pending",
            Subtotal = 100m,
            Total = 100m,
            ShippingAmount = 0m,
            PaymentMethod = "cod",
            PaymentStatus = "unpaid",
            FulfillmentStatus = "unfulfilled",
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new DeleteShippingZoneHandler(context);
        var command = new DeleteShippingZoneCommand(zone.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("associated with orders", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
