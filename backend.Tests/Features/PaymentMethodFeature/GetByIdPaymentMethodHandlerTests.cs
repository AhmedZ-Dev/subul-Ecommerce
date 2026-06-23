using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Features.PaymentMethodFeature.GetByIdPaymentMethod;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class GetByIdPaymentMethodHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedPaymentMethodAsync()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(context);
        var result = await handler.Handle(
            new CreatePaymentMethodCommand($"get_{Guid.NewGuid():N}", LabelEn: "COD", Type: "offline"),
            CancellationToken.None);

        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsSuccess()
    {
        var id = await SeedPaymentMethodAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdPaymentMethodHandler(context);

        var result = await handler.Handle(new GetByIdPaymentMethodQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value!.Id);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdPaymentMethodHandler(context);

        var result = await handler.Handle(new GetByIdPaymentMethodQuery(999999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
