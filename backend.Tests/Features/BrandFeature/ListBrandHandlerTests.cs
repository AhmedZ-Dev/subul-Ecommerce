using backend.Features.BrandFeature.CreateBrand;
using backend.Features.BrandFeature.ListBrandPaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class ListBrandHandlerTests(DatabaseFixture fixture)
{
    private async Task SeedBrandAsync(string name, string? descriptionAr = null, bool isActive = true, bool isFeatured = false)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateBrandHandler(ctx);
        await handler.Handle(
            new CreateBrandCommand(name, DescriptionAr: descriptionAr, IsActive: isActive, IsFeatured: isFeatured),
            CancellationToken.None);
    }

    [Fact]
    public async Task Handle_DefaultQuery_ReturnsPagedResult()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(new ListBrandPaginatedQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Total >= 0);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(10, result.Value.Limit);
    }

    [Fact]
    public async Task Handle_SearchByName_FiltersCorrectly()
    {
        await SeedBrandAsync("ListSearch ASUS");
        await SeedBrandAsync("ListSearch Lenovo");
        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(
            new ListBrandPaginatedQuery(Search: "ListSearch ASUS"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.Items, i => i.Name == "ListSearch ASUS");
        Assert.DoesNotContain(result.Value.Items, i => i.Name == "ListSearch Lenovo");
    }

    [Fact]
    public async Task Handle_SearchByArabicDescription_FiltersCorrectly()
    {
        await SeedBrandAsync("ListArabicSearch Brand", descriptionAr: "علامة تجارية عالمية");
        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(
            new ListBrandPaginatedQuery(Search: "عالمية"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.Items, i => i.Name == "ListArabicSearch Brand");
    }

    [Fact]
    public async Task Handle_FilterByIsActive_ReturnsOnlyActiveBrands()
    {
        await SeedBrandAsync("List Active Brand", isActive: true);
        await SeedBrandAsync("List Inactive Brand", isActive: false);
        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(
            new ListBrandPaginatedQuery(IsActive: true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item => Assert.True(item.IsActive));
    }

    [Fact]
    public async Task Handle_FilterByIsFeatured_ReturnsOnlyFeaturedBrands()
    {
        await SeedBrandAsync("List Featured Brand", isFeatured: true);
        await SeedBrandAsync("List Regular Brand", isFeatured: false);
        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(
            new ListBrandPaginatedQuery(IsFeatured: true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item => Assert.True(item.IsFeatured));
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        for (var i = 1; i <= 5; i++)
            await SeedBrandAsync($"Paging Brand {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var page1 = await handler.Handle(
            new ListBrandPaginatedQuery(Page: 1, Limit: 2),
            CancellationToken.None);

        var page2 = await handler.Handle(
            new ListBrandPaginatedQuery(Page: 2, Limit: 2),
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
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(
            new ListBrandPaginatedQuery(Page: -5),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Page);
    }

    [Fact]
    public async Task Handle_TotalPages_CalculatedCorrectly()
    {
        for (var i = 1; i <= 3; i++)
            await SeedBrandAsync($"TotalPages Brand {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListBrandPaginatedHandler(context);

        var result = await handler.Handle(
            new ListBrandPaginatedQuery(Limit: 2),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var expected = (int)Math.Ceiling(result.Value!.Total / 2.0);
        Assert.Equal(expected, result.Value.TotalPages);
    }
}
