using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeFeature.ListAttributePaginated;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AttributeFeature;

[Collection("Database")]
public class ListAttributeHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingGroup_ReturnsPaginatedAttributes()
    {
        await using var context = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"List Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "RAM", SortOrder: 1),
            CancellationToken.None);
        await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "Storage", SortOrder: 2),
            CancellationToken.None);

        var handler = new ListAttributePaginatedHandler(context);
        var result = await handler.Handle(
            new ListAttributePaginatedQuery(group.Value.Id, Page: 1, Limit: 10),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Total);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_NonExistentGroup_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new ListAttributePaginatedHandler(context);

        var result = await handler.Handle(
            new ListAttributePaginatedQuery(999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SearchFilter_ReturnsMatchingAttributes()
    {
        await using var context = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Search Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(context);
        await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Display Size"),
            CancellationToken.None);
        await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "Touch Screen"),
            CancellationToken.None);

        var handler = new ListAttributePaginatedHandler(context);
        var result = await handler.Handle(
            new ListAttributePaginatedQuery(group.Value.Id, Search: "Touch"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Total);
        Assert.Equal("Touch Screen", result.Value.Items[0].NameEn);
    }
}
