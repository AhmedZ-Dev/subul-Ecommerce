using backend.Features.CategoryFeature.CreateCategory;
using backend.Features.CategoryFeature.ListCategoryPaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CategoryFeature;

[Collection("Database")]
public class ListCategoryHandlerTests(DatabaseFixture fixture)
{
    private async Task SeedCategoryAsync(string nameEn, string? nameAr = null, bool isActive = true, long? parentId = null)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateCategoryHandler(ctx);
        await handler.Handle(
            new CreateCategoryCommand(nameEn, nameAr, null, null, parentId, IsActive: isActive),
            CancellationToken.None);
    }

    [Fact]
    public async Task Handle_DefaultQuery_ReturnsPagedResult()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(new ListCategoryPaginatedQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Total >= 0);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(10, result.Value.Limit);
    }

    [Fact]
    public async Task Handle_SearchByEnglishName_FiltersCorrectly()
    {
        await SeedCategoryAsync("ListSearch Phones");
        await SeedCategoryAsync("ListSearch Tablets");
        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCategoryPaginatedQuery(Search: "ListSearch Phones"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Items.Any(i => i.NameEn == "ListSearch Phones"));
        Assert.False(result.Value.Items.Any(i => i.NameEn == "ListSearch Tablets"));
    }

    [Fact]
    public async Task Handle_SearchByArabicName_FiltersCorrectly()
    {
        await SeedCategoryAsync("ListArabicSearch Cat", "هواتف محمولة");
        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCategoryPaginatedQuery(Search: "هواتف"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Items.Any(i => i.NameAr == "هواتف محمولة"));
    }

    [Fact]
    public async Task Handle_FilterByIsActive_ReturnsOnlyActiveCategories()
    {
        await SeedCategoryAsync("List Active Category", isActive: true);
        await SeedCategoryAsync("List Inactive Category", isActive: false);
        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCategoryPaginatedQuery(IsActive: true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item => Assert.True(item.IsActive));
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        for (var i = 1; i <= 5; i++)
            await SeedCategoryAsync($"Paging Category {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var page1 = await handler.Handle(
            new ListCategoryPaginatedQuery(Page: 1, Limit: 2),
            CancellationToken.None);

        var page2 = await handler.Handle(
            new ListCategoryPaginatedQuery(Page: 2, Limit: 2),
            CancellationToken.None);

        Assert.True(page1.IsSuccess);
        Assert.True(page2.IsSuccess);
        Assert.Equal(2, page1.Value!.Items.Count);
        Assert.Equal(1, page1.Value.Page);
        Assert.Equal(2, page2.Value!.Page);
        Assert.False(page1.Value.Items.Select(i => i.Id).Intersect(page2.Value.Items.Select(i => i.Id)).Any());
    }

    [Fact]
    public async Task Handle_InvalidPage_DefaultsToPage1()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCategoryPaginatedQuery(Page: -5),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Page);
    }

    [Fact]
    public async Task Handle_TotalPages_CalculatedCorrectly()
    {
        for (var i = 1; i <= 3; i++)
            await SeedCategoryAsync($"TotalPages Cat {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCategoryPaginatedQuery(Limit: 2),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var expected = (int)Math.Ceiling(result.Value!.Total / 2.0);
        Assert.Equal(expected, result.Value.TotalPages);
    }

    [Fact]
    public async Task Handle_FilterByParentId_ReturnsOnlyChildren()
    {
        await using var parentCtx = fixture.CreateContext();
        var parentHandler = new CreateCategoryHandler(parentCtx);
        var parent = await parentHandler.Handle(
            new CreateCategoryCommand($"List Parent {Guid.NewGuid():N}", null, null, null, null),
            CancellationToken.None);
        var parentId = parent.Value!.Id;

        await SeedCategoryAsync($"Child Of {parentId} A", parentId: parentId);
        await SeedCategoryAsync($"Child Of {parentId} B", parentId: parentId);

        await using var context = fixture.CreateContext();
        var handler = new ListCategoryPaginatedHandler(context);

        var result = await handler.Handle(
            new ListCategoryPaginatedQuery(ParentId: parentId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item => Assert.Equal(parentId, item.ParentId));
    }
}
