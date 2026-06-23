using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AttributeGroupFeature.UpdateAttributeGroup;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Tests.Features.AttributeGroupFeature;

[Collection("Database")]
public class UpdateAttributeGroupHandlerTests(DatabaseFixture fixture)
{
    private async Task<AttributeGroup> CreateGroupAsync(AppDbContext context, string name)
    {
        var group = new AttributeGroup
        {
            NameEn = name,
            Slug = name.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.Now
        };
        context.AttributeGroups.Add(group);
        await context.SaveChangesAsync();
        return group;
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesFieldsAndSyncsAttributes()
    {
        await using var context = fixture.CreateContext();
        var group = await CreateGroupAsync(context, "Old Group " + Guid.NewGuid());
        var attr = new AttributeEntity
        {
            NameEn = "Old Attr",
            Slug = "old-attr-" + Guid.NewGuid().ToString("N"),
            InputType = "text",
            CreatedAt = DateTime.Now
        };
        group.Attributes.Add(attr);
        await context.SaveChangesAsync();

        var handler = new UpdateAttributeGroupHandler(context);
        var command = new UpdateAttributeGroupCommand(
            Id: group.Id,
            NameEn: "New Group Name " + Guid.NewGuid(),
            NameAr: "مجموعة جديدة",
            Slug: "new-group-slug",
            SortOrder: 2,
            IsFilterable: false,
            Attributes: new List<UpdateAttributeGroupAttributeInput>
            {
                new(attr.Id, "Updated Attr Name", null, null, null, "select", true, 5),
                new(null, "New Added Attr", null, null, null, "text", false, 10)
            }
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(command.NameEn, result.Value.NameEn);
        Assert.Equal("new-group-slug", result.Value.Slug);
        Assert.Equal(2, result.Value.Attributes.Count);
        Assert.Equal(attr.Id, result.Value.Attributes[0].Id);
        Assert.Equal("Updated Attr Name", result.Value.Attributes[0].NameEn);
        Assert.Equal("select", result.Value.Attributes[0].InputType);
        Assert.True(result.Value.Attributes[1].Id > 0);
        Assert.Equal("New Added Attr", result.Value.Attributes[1].NameEn);
    }

    [Fact]
    public async Task Handle_DeletesAttribute_WhenNotInCommandAttributes()
    {
        await using var context = fixture.CreateContext();
        var group = await CreateGroupAsync(context, "Delete Attribute Sync Group " + Guid.NewGuid());
        var attr = new AttributeEntity
        {
            NameEn = "Delete Me",
            Slug = "delete-me-" + Guid.NewGuid().ToString("N"),
            InputType = "text",
            CreatedAt = DateTime.Now
        };
        group.Attributes.Add(attr);
        await context.SaveChangesAsync();

        var handler = new UpdateAttributeGroupHandler(context);
        var command = new UpdateAttributeGroupCommand(
            Id: group.Id,
            NameEn: group.NameEn,
            NameAr: null,
            Slug: null,
            SortOrder: 0,
            IsFilterable: true,
            Attributes: new List<UpdateAttributeGroupAttributeInput>() // Empty list → deletes existing
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Attributes);
    }

    [Fact]
    public async Task Handle_DeleteAttributeWithSpecsGuard_ReturnsFailure()
    {
        await using var context = fixture.CreateContext();
        var group = await CreateGroupAsync(context, "Group With Spec " + Guid.NewGuid());
        var attr = new AttributeEntity
        {
            NameEn = "Linked Spec Attribute",
            Slug = "linked-spec-attr-" + Guid.NewGuid().ToString("N"),
            InputType = "text",
            CreatedAt = DateTime.Now
        };
        group.Attributes.Add(attr);
        await context.SaveChangesAsync();

        // Create product
        var product = new Product
        {
            NameEn = "Laptop XYZ",
            Slug = "laptop-xyz-" + Guid.NewGuid().ToString("N"),
            Status = "active",
            Price = 1000m,
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Link with value
        var val = new ProductAttributeValue
        {
            ProductId = product.Id,
            AttributeId = attr.Id,
            ValueText = "Intel i7",
            CreatedAt = DateTime.Now
        };
        context.ProductAttributeValues.Add(val);
        await context.SaveChangesAsync();

        var handler = new UpdateAttributeGroupHandler(context);
        var command = new UpdateAttributeGroupCommand(
            Id: group.Id,
            NameEn: group.NameEn,
            NameAr: null,
            Slug: null,
            SortOrder: 0,
            IsFilterable: true,
            Attributes: new List<UpdateAttributeGroupAttributeInput>() // Trying to delete by sending empty list
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("associated with product specifications", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
