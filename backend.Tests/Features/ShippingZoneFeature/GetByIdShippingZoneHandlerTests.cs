using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.ShippingZoneFeature.GetByIdShippingZone;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.ShippingZoneFeature;

[Collection("Database")]
public class GetByIdShippingZoneHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingId_ReturnsShippingZone()
    {
        await using var context = fixture.CreateContext();
        var zone = new ShippingZone
        {
            NameEn = "North Zone " + Guid.NewGuid(),
            Governorates = JsonSerializer.Serialize(new List<string> { "Erbil", "Duhok" }),
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        zone.ShippingRates.Add(new ShippingRate
        {
            NameEn = "Express Delivery",
            RateType = "flat",
            Price = 10000m,
            IsActive = true,
            CreatedAt = DateTime.Now
        });

        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync();

        var handler = new GetByIdShippingZoneHandler(context);
        var query = new GetByIdShippingZoneQuery(zone.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(zone.NameEn, result.Value.NameEn);
        Assert.Equal(2, result.Value.Governorates.Count);
        Assert.Single(result.Value.ShippingRates);
        Assert.Equal("Express Delivery", result.Value.ShippingRates[0].NameEn);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdShippingZoneHandler(context);
        var query = new GetByIdShippingZoneQuery(999999);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
