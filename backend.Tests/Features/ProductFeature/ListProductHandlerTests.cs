using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductFeature.ListProductPaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

[Collection("Database")]
public class ListProductHandlerTests(DatabaseFixture fixture)
{
    private async Task SeedProductAsync(string nameEn, string? nameAr = null, string status = "active", bool isFeatured = false)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateProductHandler(ctx);
        await handler.Handle(
            new CreateProductCommand(nameEn, nameAr, null, null, Price: 100, Status: status, IsFeatured: isFeatured),
            CancellationToken.None);
    }

    [Fact]
    public async Task Handle_DefaultQuery_ReturnsPagedResult()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(new ListProductPaginatedQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Total >= 0);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(10, result.Value.Limit);
    }

    [Fact]
    public async Task Handle_SearchByEnglishName_FiltersCorrectly()
    {
        await SeedProductAsync("ListSearch Laptop Pro");
        await SeedProductAsync("ListSearch Phone Max");
        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(Search: "ListSearch Laptop"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Items.Any(i => i.NameEn == "ListSearch Laptop Pro"));
        Assert.False(result.Value.Items.Any(i => i.NameEn == "ListSearch Phone Max"));
    }

    [Fact]
    public async Task Handle_SearchByArabicName_FiltersCorrectly()
    {
        await SeedProductAsync("ListArabicSearch Product", "هاتف ذكي");
        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(Search: "هاتف"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Items.Any(i => i.NameAr == "هاتف ذكي"));
    }

    [Fact]
    public async Task Handle_FilterByStatus_ReturnsOnlyMatchingProducts()
    {
        await SeedProductAsync("List Active Product", status: "active");
        await SeedProductAsync("List Draft Product", status: "draft");
        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(Status: "draft"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item => Assert.Equal("draft", item.Status));
    }

    [Fact]
    public async Task Handle_FilterByIsFeatured_ReturnsOnlyFeaturedProducts()
    {
        await SeedProductAsync("List Featured Product", isFeatured: true);
        await SeedProductAsync("List Regular Product", isFeatured: false);
        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(IsFeatured: true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item => Assert.True(item.IsFeatured));
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        for (var i = 1; i <= 5; i++)
            await SeedProductAsync($"Paging Product {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var page1 = await handler.Handle(
            new ListProductPaginatedQuery(Page: 1, Limit: 2),
            CancellationToken.None);

        var page2 = await handler.Handle(
            new ListProductPaginatedQuery(Page: 2, Limit: 2),
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
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(Page: -5),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Page);
    }

    [Fact]
    public async Task Handle_TotalPages_CalculatedCorrectly()
    {
        for (var i = 1; i <= 3; i++)
            await SeedProductAsync($"TotalPages Product {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(Limit: 2),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var expected = (int)Math.Ceiling(result.Value!.Total / 2.0);
        Assert.Equal(expected, result.Value.TotalPages);
    }
}
