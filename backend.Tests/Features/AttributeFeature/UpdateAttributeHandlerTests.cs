using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeFeature.UpdateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AttributeFeature;

[Collection("Database")]
public class UpdateAttributeHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long GroupId, long AttributeId)> SeedAttributeAsync(string slug)
    {
        await using var ctx = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Update Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(
                group.Value!.Id,
                NameEn: "Original",
                Slug: slug,
                InputType: "text",
                SortOrder: 0),
            CancellationToken.None);

        return (group.Value.Id, attribute.Value!.Id);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var (groupId, attributeId) = await SeedAttributeAsync($"upd-{Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new UpdateAttributeHandler(context);
        var command = new UpdateAttributeCommand(
            groupId,
            attributeId,
            NameEn: "Updated Attribute",
            NameAr: null,
            Slug: $"upd-new-{Guid.NewGuid():N}",
            Unit: "GB",
            InputType: "number",
            IsFilterable: false,
            SortOrder: 2);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Attribute", result.Value!.NameEn);
        Assert.Equal("number", result.Value.InputType);
        Assert.Equal(2, result.Value.SortOrder);
    }

    [Fact]
    public async Task Handle_NonExistentAttribute_ReturnsNotFound()
    {
        var (groupId, _) = await SeedAttributeAsync($"nf-{Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new UpdateAttributeHandler(context);
        var command = new UpdateAttributeCommand(
            groupId,
            999999,
            NameEn: "Ghost",
            NameAr: null,
            Slug: null,
            Unit: null,
            InputType: "text",
            IsFilterable: true,
            SortOrder: 0);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateSlug_AppendsNumberSuffix()
    {
        var slugA = $"slug-a-{Guid.NewGuid():N}";
        var slugB = $"slug-b-{Guid.NewGuid():N}";

        await using var context = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(context);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Dup Slug Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var createHandler = new CreateAttributeHandler(context);
        var attributeA = await createHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "A", Slug: slugA),
            CancellationToken.None);
        await createHandler.Handle(
            new CreateAttributeCommand(group.Value.Id, NameEn: "B", Slug: slugB),
            CancellationToken.None);

        var updateHandler = new UpdateAttributeHandler(context);
        var result = await updateHandler.Handle(
            new UpdateAttributeCommand(
                group.Value.Id,
                attributeA.Value!.Id,
                NameEn: "A Updated",
                NameAr: null,
                Slug: slugB,
                Unit: null,
                InputType: "text",
                IsFilterable: true,
                SortOrder: 0),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal($"{slugB}-2", result.Value!.Slug);
    }
}
