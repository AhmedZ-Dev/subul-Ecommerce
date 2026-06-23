using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.CollectionFeature.ListCollectionPaginated;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.CollectionFeature;

[Collection("Database")]
public class ListCollectionHandlerTests(DatabaseFixture fixture)
{
    private async Task<Collection> CreateCollectionAsync(AppDbContext context, string name, string type, bool isActive)
    {
        var collection = new Collection
        {
            NameEn = name,
            Slug = name.ToLower().Replace(" ", "-").Replace("&", "and") + "-" + Guid.NewGuid().ToString("N"),
            CollectionType = type,
            IsActive = isActive,
            CreatedAt = DateTime.Now
        };
        context.Collections.Add(collection);
        await context.SaveChangesAsync();
        return collection;
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllCollections()
    {
        await using var context = fixture.CreateContext();
        await CreateCollectionAsync(context, "List Col A " + Guid.NewGuid(), "manual", true);
        await CreateCollectionAsync(context, "List Col B " + Guid.NewGuid(), "smart", true);

        var handler = new ListCollectionPaginatedHandler(context);
        var query = new ListCollectionPaginatedQuery(Page: 1, Limit: 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Items.Count >= 2);
    }

    [Fact]
    public async Task Handle_FilterBySearch_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var uniqueSearch = "UniqueSearchKey_" + Guid.NewGuid().ToString("N");
        await CreateCollectionAsync(context, "Matching " + uniqueSearch, "manual", true);
        await CreateCollectionAsync(context, "Non Matching Col", "manual", true);

        var handler = new ListCollectionPaginatedHandler(context);
        var query = new ListCollectionPaginatedQuery(Search: uniqueSearch);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Contains(uniqueSearch, result.Value.Items[0].NameEn);
    }

    [Fact]
    public async Task Handle_FilterByIsActive_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var suffix = Guid.NewGuid().ToString("N");
        var activeCol = await CreateCollectionAsync(context, "Active " + suffix, "manual", true);
        var inactiveCol = await CreateCollectionAsync(context, "Inactive " + suffix, "manual", false);

        var handler = new ListCollectionPaginatedHandler(context);
        var query = new ListCollectionPaginatedQuery(IsActive: true, Search: suffix);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(activeCol.Id, result.Value.Items[0].Id);
    }

    [Fact]
    public async Task Handle_FilterByCollectionType_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var suffix = Guid.NewGuid().ToString("N");
        var manualCol = await CreateCollectionAsync(context, "Manual " + suffix, "manual", true);
        var smartCol = await CreateCollectionAsync(context, "Smart " + suffix, "smart", true);

        var handler = new ListCollectionPaginatedHandler(context);
        var query = new ListCollectionPaginatedQuery(CollectionType: "smart", Search: suffix);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(smartCol.Id, result.Value.Items[0].Id);
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectSubset()
    {
        await using var context = fixture.CreateContext();
        var suffix = Guid.NewGuid().ToString("N");
        for (int i = 0; i < 5; i++)
        {
            await CreateCollectionAsync(context, $"Col {i} " + suffix, "manual", true);
        }

        var handler = new ListCollectionPaginatedHandler(context);
        var query = new ListCollectionPaginatedQuery(Page: 2, Limit: 2, Search: suffix);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Items.Count);
        Assert.Equal(5, result.Value.Total);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.Equal(2, result.Value.Page);
    }
}
