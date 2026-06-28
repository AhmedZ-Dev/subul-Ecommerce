using backend.Features.CategoryFeature.ChangeCategoryStatus;
using backend.Features.CategoryFeature.CreateCategory;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CategoryFeature;

[Collection("Database")]
public class ChangeCategoryStatusHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedCategoryAsync(string nameEn)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateCategoryHandler(ctx);
        var result = await handler.Handle(
            new CreateCategoryCommand(nameEn, null, null, null, null),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_DeactivateCategory_ReturnsSuccess()
    {
        var id = await SeedCategoryAsync($"Status Deactivate {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new ChangeCategoryStatusHandler(context);

        var result = await handler.Handle(
            new ChangeCategoryStatusCommand(id, false),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value!.Id);
        Assert.False(result.Value.IsActive);
        Assert.NotNull(result.Value.UpdatedAt);
    }

    [Fact]
    public async Task Handle_ActivateCategory_ReturnsSuccess()
    {
        var id = await SeedCategoryAsync($"Status Activate {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var deactivateHandler = new ChangeCategoryStatusHandler(context);
        await deactivateHandler.Handle(
            new ChangeCategoryStatusCommand(id, false),
            CancellationToken.None);

        var handler = new ChangeCategoryStatusHandler(context);
        var result = await handler.Handle(
            new ChangeCategoryStatusCommand(id, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsActive);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ChangeCategoryStatusHandler(context);

        var result = await handler.Handle(
            new ChangeCategoryStatusCommand(999999, false),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
