using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AttributeGroupFeature.ListAttributeGroupPaginated;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AttributeGroupFeature;

[Collection("Database")]
public class ListAttributeGroupHandlerTests(DatabaseFixture fixture)
{
    private async Task<AttributeGroup> CreateGroupAsync(AppDbContext context, string name, bool isFilterable)
    {
        var group = new AttributeGroup
        {
            NameEn = name,
            Slug = name.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString("N"),
            IsFilterable = isFilterable,
            CreatedAt = DateTime.Now
        };
        context.AttributeGroups.Add(group);
        await context.SaveChangesAsync();
        return group;
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAll()
    {
        await using var context = fixture.CreateContext();
        await CreateGroupAsync(context, "Group A " + Guid.NewGuid(), true);
        await CreateGroupAsync(context, "Group B " + Guid.NewGuid(), false);

        var handler = new ListAttributeGroupPaginatedHandler(context);
        var query = new ListAttributeGroupPaginatedQuery(Page: 1, Limit: 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Items.Count >= 2);
    }

    [Fact]
    public async Task Handle_FilterBySearch_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var key = "SpecialKey_" + Guid.NewGuid().ToString("N");
        await CreateGroupAsync(context, "Name " + key, true);
        await CreateGroupAsync(context, "Name Other Group", true);

        var handler = new ListAttributeGroupPaginatedHandler(context);
        var query = new ListAttributeGroupPaginatedQuery(Search: key);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Contains(key, result.Value.Items[0].NameEn);
    }

    [Fact]
    public async Task Handle_FilterByIsFilterable_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var suffix = Guid.NewGuid().ToString("N");
        var filterableGroup = await CreateGroupAsync(context, "Filterable " + suffix, true);
        var unfilterableGroup = await CreateGroupAsync(context, "Unfilterable " + suffix, false);

        var handler = new ListAttributeGroupPaginatedHandler(context);
        var query = new ListAttributeGroupPaginatedQuery(IsFilterable: true, Search: suffix);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(filterableGroup.Id, result.Value.Items[0].Id);
    }
}
