using backend.Features.CategoryFeature.CreateCategory;
using backend.Features.CategoryFeature.GetByIdCategory;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CategoryFeature;

[Collection("Database")]
public class GetByIdCategoryHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedCategoryAsync(string nameEn, long? parentId = null)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateCategoryHandler(ctx);
        var result = await handler.Handle(
            new CreateCategoryCommand(nameEn, null, null, null, parentId),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsCategoryWithCorrectData()
    {
        var id = await SeedCategoryAsync("GetById Target");
        await using var context = fixture.CreateContext();
        var handler = new GetByIdCategoryHandler(context);

        var result = await handler.Handle(new GetByIdCategoryQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("GetById Target", result.Value.NameEn);
        Assert.Null(result.Value.Parent);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdCategoryHandler(context);

        var result = await handler.Handle(new GetByIdCategoryQuery(999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_CategoryWithParent_ReturnsParentInfo()
    {
        var parentId = await SeedCategoryAsync("GetById Parent");
        var childId = await SeedCategoryAsync("GetById Child", parentId);

        await using var context = fixture.CreateContext();
        var handler = new GetByIdCategoryHandler(context);

        var result = await handler.Handle(new GetByIdCategoryQuery(childId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Parent);
        Assert.Equal(parentId, result.Value.Parent.Id);
        Assert.Equal("GetById Parent", result.Value.Parent.NameEn);
    }

    [Fact]
    public async Task Handle_ReturnsCounts_AsZeroForNewCategory()
    {
        var id = await SeedCategoryAsync("GetById Counts Test");
        await using var context = fixture.CreateContext();
        var handler = new GetByIdCategoryHandler(context);

        var result = await handler.Handle(new GetByIdCategoryQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!._count.Product);
        Assert.Equal(0, result.Value._count.SubCategory);
    }
}
