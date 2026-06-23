using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Features.PaymentMethodFeature.ListPaymentMethodPaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class ListPaymentMethodHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResults()
    {
        await using var ctx = fixture.CreateContext();
        var createHandler = new CreatePaymentMethodHandler(ctx);
        await createHandler.Handle(
            new CreatePaymentMethodCommand($"list_a_{Guid.NewGuid():N}", Type: "offline"),
            CancellationToken.None);
        await createHandler.Handle(
            new CreatePaymentMethodCommand($"list_b_{Guid.NewGuid():N}", Type: "online"),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListPaymentMethodPaginatedHandler(context);

        var result = await handler.Handle(
            new ListPaymentMethodPaginatedQuery(Page: 1, Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Total >= 2);
        Assert.NotEmpty(result.Value.Items);
    }

    [Fact]
    public async Task Handle_SearchByLabel_FiltersResults()
    {
        await using var ctx = fixture.CreateContext();
        var createHandler = new CreatePaymentMethodHandler(ctx);
        var uniqueLabel = $"UniqueLabel{Guid.NewGuid():N}";
        await createHandler.Handle(
            new CreatePaymentMethodCommand($"search_{Guid.NewGuid():N}", LabelEn: uniqueLabel, Type: "offline"),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListPaymentMethodPaginatedHandler(context);

        var result = await handler.Handle(
            new ListPaymentMethodPaginatedQuery(Search: uniqueLabel),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.Items, i => i.LabelEn == uniqueLabel);
    }

    [Fact]
    public async Task Handle_FilterByType_ReturnsMatchingItems()
    {
        await using var ctx = fixture.CreateContext();
        var createHandler = new CreatePaymentMethodHandler(ctx);
        await createHandler.Handle(
            new CreatePaymentMethodCommand($"offline_{Guid.NewGuid():N}", Type: "offline"),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListPaymentMethodPaginatedHandler(context);

        var result = await handler.Handle(
            new ListPaymentMethodPaginatedQuery(Type: "offline"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, i => Assert.Equal("offline", i.Type));
    }
}
