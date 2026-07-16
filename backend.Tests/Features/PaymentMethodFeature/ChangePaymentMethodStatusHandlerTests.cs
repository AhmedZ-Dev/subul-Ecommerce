using backend.Features.PaymentMethodFeature.ChangePaymentMethodStatus;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class ChangePaymentMethodStatusHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedPaymentMethodAsync()
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(ctx);
        var result = await handler.Handle(
            new CreatePaymentMethodCommand($"status_{Guid.NewGuid():N}", LabelEn: "Status Test", Type: "offline"),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_DeactivatePaymentMethod_ReturnsSuccess()
    {
        var id = await SeedPaymentMethodAsync();
        await using var context = fixture.CreateContext();
        var handler = new ChangePaymentMethodStatusHandler(context);

        var result = await handler.Handle(
            new ChangePaymentMethodStatusCommand(id, false),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value!.Id);
        Assert.False(result.Value.IsActive);
        Assert.NotNull(result.Value.UpdatedAt);
    }

    [Fact]
    public async Task Handle_ActivatePaymentMethod_ReturnsSuccess()
    {
        var id = await SeedPaymentMethodAsync();
        await using var context = fixture.CreateContext();
        var deactivateHandler = new ChangePaymentMethodStatusHandler(context);
        await deactivateHandler.Handle(
            new ChangePaymentMethodStatusCommand(id, false),
            CancellationToken.None);

        var handler = new ChangePaymentMethodStatusHandler(context);
        var result = await handler.Handle(
            new ChangePaymentMethodStatusCommand(id, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsActive);
    }

    [Fact]
    public async Task Handle_PaymentMethodNotFound_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ChangePaymentMethodStatusHandler(context);

        var result = await handler.Handle(
            new ChangePaymentMethodStatusCommand(999999, false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
