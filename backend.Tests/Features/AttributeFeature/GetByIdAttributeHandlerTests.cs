using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeFeature.GetByIdAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AttributeFeature;

[Collection("Database")]
public class GetByIdAttributeHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long GroupId, long AttributeId)> SeedAttributeAsync()
    {
        await using var ctx = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"GetById Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Processor", InputType: "text"),
            CancellationToken.None);

        return (group.Value.Id, attribute.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingAttribute_ReturnsSuccess()
    {
        var (groupId, attributeId) = await SeedAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdAttributeHandler(context);

        var result = await handler.Handle(
            new GetByIdAttributeQuery(groupId, attributeId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(attributeId, result.Value!.Id);
        Assert.Equal(groupId, result.Value.GroupId);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        var (groupId, _) = await SeedAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdAttributeHandler(context);

        var result = await handler.Handle(
            new GetByIdAttributeQuery(groupId, 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WrongGroupId_ReturnsNotFound()
    {
        var (groupId, attributeId) = await SeedAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetByIdAttributeHandler(context);

        var result = await handler.Handle(
            new GetByIdAttributeQuery(groupId + 999, attributeId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
