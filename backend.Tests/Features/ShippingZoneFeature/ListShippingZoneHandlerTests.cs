using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.ShippingZoneFeature.ListShippingZonePaginated;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.ShippingZoneFeature;

[Collection("Database")]
public class ListShippingZoneHandlerTests(DatabaseFixture fixture)
{
    private async Task<ShippingZone> CreateZoneAsync(AppDbContext context, string name, bool isActive, List<string>? governorates = null)
    {
        var zone = new ShippingZone
        {
            NameEn = name,
            Governorates = governorates is not null ? JsonSerializer.Serialize(governorates) : null,
            IsActive = isActive,
            CreatedAt = DateTime.Now
        };
        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync();
        return zone;
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAll()
    {
        await using var context = fixture.CreateContext();
        await CreateZoneAsync(context, "Zone A " + Guid.NewGuid(), true);
        await CreateZoneAsync(context, "Zone B " + Guid.NewGuid(), false);

        var handler = new ListShippingZonePaginatedHandler(context);
        var query = new ListShippingZonePaginatedQuery(Page: 1, Limit: 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Items.Count >= 2);
    }

    [Fact]
    public async Task Handle_FilterBySearch_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var key = "SpecialZoneKey_" + Guid.NewGuid().ToString("N");
        await CreateZoneAsync(context, "Search Zone " + key, true);
        await CreateZoneAsync(context, "Search Zone Other", true);

        var handler = new ListShippingZonePaginatedHandler(context);
        var query = new ListShippingZonePaginatedQuery(Search: key);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Contains(key, result.Value.Items[0].NameEn);
    }

    [Fact]
    public async Task Handle_FilterByIsActive_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var suffix = Guid.NewGuid().ToString("N");
        var active = await CreateZoneAsync(context, "Active " + suffix, true);
        var inactive = await CreateZoneAsync(context, "Inactive " + suffix, false);

        var handler = new ListShippingZonePaginatedHandler(context);
        var query = new ListShippingZonePaginatedQuery(IsActive: true, Search: suffix);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(active.Id, result.Value.Items[0].Id);
    }
}
