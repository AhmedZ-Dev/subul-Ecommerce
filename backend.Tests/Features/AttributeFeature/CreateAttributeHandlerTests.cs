using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AttributeFeature;

[Collection("Database")]
public class CreateAttributeHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedGroupAsync(string nameEn)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateAttributeGroupHandler(ctx);
        var result = await handler.Handle(
            new CreateAttributeGroupCommand(nameEn),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var groupId = await SeedGroupAsync($"Attr Group {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeHandler(context);
        var nameEn = $"RAM {Guid.NewGuid():N}";
        var command = new CreateAttributeCommand(
            groupId,
            NameEn: nameEn,
            NameAr: "الذاكرة",
            Unit: "GB",
            InputType: "select",
            IsFilterable: true,
            SortOrder: 1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(groupId, result.Value.GroupId);
        Assert.Equal(nameEn, result.Value.NameEn);
        Assert.StartsWith("ram-", result.Value.Slug, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("select", result.Value.InputType);
        Assert.True(result.Value.Id > 0);
    }

    [Fact]
    public async Task Handle_NonExistentGroupId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeHandler(context);
        var command = new CreateAttributeCommand(999999, NameEn: "Orphan Attribute");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateSlug_AppendsNumberSuffix()
    {
        var groupId = await SeedGroupAsync($"Slug Group {Guid.NewGuid():N}");
        var slug = $"attr-slug-{Guid.NewGuid():N}";

        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeHandler(context);

        var first = await handler.Handle(
            new CreateAttributeCommand(groupId, NameEn: "Attribute A", Slug: slug),
            CancellationToken.None);

        var second = await handler.Handle(
            new CreateAttributeCommand(groupId, NameEn: "Attribute B", Slug: slug),
            CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(slug, first.Value!.Slug);
        Assert.Equal($"{slug}-2", second.Value!.Slug);
    }

    [Fact]
    public async Task Handle_InvalidInputType_ReturnsBadRequest()
    {
        var groupId = await SeedGroupAsync($"Invalid Type Group {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeHandler(context);
        var command = new CreateAttributeCommand(groupId, NameEn: "Bad Type", InputType: "invalid");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("input type", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InputWithWhitespace_TrimsFields()
    {
        var groupId = await SeedGroupAsync($"Trim Group {Guid.NewGuid():N}");
        await using var context = fixture.CreateContext();
        var handler = new CreateAttributeHandler(context);
        var command = new CreateAttributeCommand(
            groupId,
            NameEn: "  GPU Memory  ",
            NameAr: "  ذاكرة  ",
            Unit: "  GB  ");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("GPU Memory", result.Value!.NameEn);
        Assert.Equal("ذاكرة", result.Value.NameAr);
        Assert.Equal("GB", result.Value.Unit);
    }
}
