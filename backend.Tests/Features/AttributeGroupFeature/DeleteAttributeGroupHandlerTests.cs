using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AttributeGroupFeature.DeleteAttributeGroup;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AttributeEntity = backend.Domain.Entities.Attribute;

namespace backend.Tests.Features.AttributeGroupFeature;

[Collection("Database")]
public class DeleteAttributeGroupHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ExistingId_DeletesGroupAndAttributes()
    {
        await using var context = fixture.CreateContext();
        var group = await CreateGroupAsync(context, "Group for Delete " + Guid.NewGuid());
        var attr = new AttributeEntity
        {
            NameEn = "Attr for Delete",
            Slug = "attr-for-delete-" + Guid.NewGuid().ToString("N"),
            InputType = "text",
            CreatedAt = DateTime.Now
        };
        group.Attributes.Add(attr);
        await context.SaveChangesAsync();

        var handler = new DeleteAttributeGroupHandler(context);
        var command = new DeleteAttributeGroupCommand(group.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);

        var deletedGroup = await context.AttributeGroups.AnyAsync(g => g.Id == group.Id);
        Assert.False(deletedGroup);

        var deletedAttr = await context.Attributes.AnyAsync(a => a.Id == attr.Id);
        Assert.False(deletedAttr);
    }

    [Fact]
    public async Task Handle_GroupWithProductSpecs_ReturnsFailure()
    {
        await using var context = fixture.CreateContext();
        var group = await CreateGroupAsync(context, "Group for Delete Fail " + Guid.NewGuid());
        var attr = new AttributeEntity
        {
            NameEn = "Attr with Spec value",
            Slug = "attr-with-spec-val-" + Guid.NewGuid().ToString("N"),
            InputType = "text",
            CreatedAt = DateTime.Now
        };
        group.Attributes.Add(attr);
        await context.SaveChangesAsync();

        // Create product
        var product = new Product
        {
            NameEn = "Phone 123",
            Slug = "phone-123-" + Guid.NewGuid().ToString("N"),
            Status = "active",
            Price = 500m,
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
            ValueText = "OLED",
            CreatedAt = DateTime.Now
        };
        context.ProductAttributeValues.Add(val);
        await context.SaveChangesAsync();

        var handler = new DeleteAttributeGroupHandler(context);
        var command = new DeleteAttributeGroupCommand(group.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("associated with product specifications", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
