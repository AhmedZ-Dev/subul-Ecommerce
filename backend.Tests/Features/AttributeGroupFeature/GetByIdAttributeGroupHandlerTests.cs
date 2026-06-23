using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AttributeGroupFeature.GetByIdAttributeGroup;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Tests.Features.AttributeGroupFeature;

[Collection("Database")]
public class GetByIdAttributeGroupHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_ExistingId_ReturnsAttributeGroup()
    {
        await using var context = fixture.CreateContext();
        var group = new AttributeGroup
        {
            NameEn = "Specs Group " + Guid.NewGuid(),
            Slug = "specs-group-" + Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.Now
        };
        group.Attributes.Add(new AttributeEntity
        {
            NameEn = "Attr One",
            Slug = "attr-one-" + Guid.NewGuid().ToString("N"),
            InputType = "text",
            CreatedAt = DateTime.Now
        });

        context.AttributeGroups.Add(group);
        await context.SaveChangesAsync();

        var handler = new GetByIdAttributeGroupHandler(context);
        var query = new GetByIdAttributeGroupQuery(group.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(group.NameEn, result.Value.NameEn);
        Assert.Single(result.Value.Attributes);
        Assert.Equal("Attr One", result.Value.Attributes[0].NameEn);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdAttributeGroupHandler(context);
        var query = new GetByIdAttributeGroupQuery(999999);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
