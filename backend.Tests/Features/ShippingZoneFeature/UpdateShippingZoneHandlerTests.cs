using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.ShippingZoneFeature.UpdateShippingZone;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.ShippingZoneFeature;

[Collection("Database")]
public class UpdateShippingZoneHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ValidCommand_UpdatesFieldsAndSyncsRates()
    {
        await using var context = fixture.CreateContext();
        var zone = await CreateZoneAsync(context, "Old Zone " + Guid.NewGuid());
        var rate = new ShippingRate
        {
            NameEn = "Old Rate",
            RateType = "flat",
            Price = 5000m,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        zone.ShippingRates.Add(rate);
        await context.SaveChangesAsync();

        var handler = new UpdateShippingZoneHandler(context);
        var command = new UpdateShippingZoneCommand(
            Id: zone.Id,
            NameEn: "New Zone Name " + Guid.NewGuid(),
            NameAr: "تحديث منطقة",
            Governorates: new List<string> { "Nineveh" },
            IsActive: false,
            ShippingRates: new List<UpdateShippingRateInput>
            {
                new(rate.Id, "Updated Rate", null, "flat", 7000m),
                new(null, "New Rate Added", null, "flat", 12000m)
            }
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(command.NameEn, result.Value.NameEn);
        Assert.False(result.Value.IsActive);
        Assert.Single(result.Value.Governorates);
        Assert.Equal("Nineveh", result.Value.Governorates[0]);
        Assert.Equal(2, result.Value.ShippingRates.Count);
        Assert.Equal(rate.Id, result.Value.ShippingRates[0].Id);
        Assert.Equal("Updated Rate", result.Value.ShippingRates[0].NameEn);
        Assert.Equal(7000m, result.Value.ShippingRates[0].Price);
        Assert.True(result.Value.ShippingRates[1].Id > 0);
        Assert.Equal("New Rate Added", result.Value.ShippingRates[1].NameEn);
    }

    [Fact]
    public async Task Handle_NonExistentZone_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new UpdateShippingZoneHandler(context);
        var command = new UpdateShippingZoneCommand(
            Id: 999999,
            NameEn: "Non Existent Zone",
            NameAr: null,
            Governorates: null,
            IsActive: true,
            ShippingRates: null
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
