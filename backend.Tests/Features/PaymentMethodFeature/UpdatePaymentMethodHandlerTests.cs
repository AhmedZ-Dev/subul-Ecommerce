using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Features.PaymentMethodFeature.UpdatePaymentMethod;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class UpdatePaymentMethodHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedPaymentMethodAsync()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(context);
        var result = await handler.Handle(
            new CreatePaymentMethodCommand($"update_{Guid.NewGuid():N}", LabelEn: "Before", Type: "offline"),
            CancellationToken.None);

        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var id = await SeedPaymentMethodAsync();
        await using var context = fixture.CreateContext();
        var handler = new UpdatePaymentMethodHandler(context);

        var result = await handler.Handle(
            new UpdatePaymentMethodCommand(
                id,
                Name: $"updated_{Guid.NewGuid():N}",
                LabelEn: "After",
                LabelAr: "بعد",
                Type: "offline",
                Gateway: null,
                GatewayConfig: null,
                IconUrl: null,
                InstructionsEn: "Pay at door",
                InstructionsAr: null,
                IsActive: true,
                SortOrder: 1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("After", result.Value!.LabelEn);
        Assert.Equal(1, result.Value.SortOrder);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new UpdatePaymentMethodHandler(context);

        var result = await handler.Handle(
            new UpdatePaymentMethodCommand(
                999999999,
                Name: "missing",
                null, null, "offline", null, null, null, null, null, true, 0),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsConflict()
    {
        await using var ctx = fixture.CreateContext();
        var createHandler = new CreatePaymentMethodHandler(ctx);
        var first = await createHandler.Handle(
            new CreatePaymentMethodCommand($"first_{Guid.NewGuid():N}", Type: "offline"),
            CancellationToken.None);
        var second = await createHandler.Handle(
            new CreatePaymentMethodCommand($"second_{Guid.NewGuid():N}", Type: "offline"),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new UpdatePaymentMethodHandler(context);

        var result = await handler.Handle(
            new UpdatePaymentMethodCommand(
                second.Value!.Id,
                Name: first.Value!.Name,
                null, null, "offline", null, null, null, null, null, true, 0),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
