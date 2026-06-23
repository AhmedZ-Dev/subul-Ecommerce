using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class CreatePaymentMethodHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(context);
        var command = new CreatePaymentMethodCommand(
            Name: $"cod_{Guid.NewGuid():N}",
            LabelEn: "Cash on Delivery",
            LabelAr: "الدفع عند الاستلام",
            Type: "offline",
            IsActive: true,
            SortOrder: 0);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name.Trim().ToLowerInvariant(), result.Value!.Name);
        Assert.Equal("Cash on Delivery", result.Value.LabelEn);
        Assert.Equal("offline", result.Value.Type);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsConflict()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(context);
        var name = $"bank_{Guid.NewGuid():N}";
        var command = new CreatePaymentMethodCommand(Name: name, Type: "offline");

        await handler.Handle(command, CancellationToken.None);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InvalidType_ReturnsFailure()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(context);
        var command = new CreatePaymentMethodCommand(
            Name: $"invalid_{Guid.NewGuid():N}",
            Type: "crypto");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid payment method type", result.Error!);
    }

    [Fact]
    public async Task Handle_EmptyName_ReturnsFailure()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreatePaymentMethodHandler(context);
        var command = new CreatePaymentMethodCommand(Name: "   ");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("name is required", result.Error!, StringComparison.OrdinalIgnoreCase);
    }
}
