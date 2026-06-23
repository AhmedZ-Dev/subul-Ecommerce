using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Features.ShippingZoneFeature.CreateShippingZone;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.ShippingZoneFeature;

[Collection("Database")]
public class CreateShippingZoneHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateShippingZoneHandler(context);
        var command = new CreateShippingZoneCommand(
            NameEn: "Baghdad Zone",
            NameAr: "منطقة بغداد",
            Governorates: new List<string> { "Baghdad" },
            IsActive: true
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Baghdad Zone", result.Value.NameEn);
        Assert.Single(result.Value.Governorates);
        Assert.Equal("Baghdad", result.Value.Governorates[0]);
    }

    [Fact]
    public async Task Handle_WithRates_CreatesRates()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateShippingZoneHandler(context);
        var command = new CreateShippingZoneCommand(
            NameEn: "Southern Zone " + Guid.NewGuid(),
            Governorates: new List<string> { "Basra", "Maysan" },
            ShippingRates: new List<CreateShippingRateInput>
            {
                new("Standard Delivery", null, "flat", 5000m, null, null, null, 2, 4)
            }
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.ShippingRates);
        Assert.Equal("Standard Delivery", result.Value.ShippingRates[0].NameEn);
        Assert.Equal(5000m, result.Value.ShippingRates[0].Price);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsConflict()
    {
        await using var context = fixture.CreateContext();
        var name = "Dup Zone " + Guid.NewGuid();
        var handler = new CreateShippingZoneHandler(context);
        var command = new CreateShippingZoneCommand(NameEn: name);

        await handler.Handle(command, CancellationToken.None);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
