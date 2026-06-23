using backend.Domain.Entities;
using backend.Features.AttributeFeature.CreateAttribute;
using backend.Features.AttributeFeature.DeleteAttribute;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.ProductFeature.CreateProduct;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AttributeFeature;

[Collection("Database")]
public class DeleteAttributeHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long GroupId, long AttributeId)> SeedAttributeAsync()
    {
        await using var ctx = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand($"Delete Group {Guid.NewGuid():N}"),
            CancellationToken.None);

        var attributeHandler = new CreateAttributeHandler(ctx);
        var attribute = await attributeHandler.Handle(
            new CreateAttributeCommand(group.Value!.Id, NameEn: "Delete Me"),
            CancellationToken.None);

        return (group.Value.Id, attribute.Value!.Id);
    }

    [Fact]
    public async Task Handle_ExistingAttribute_DeletesSuccessfully()
    {
        var (groupId, attributeId) = await SeedAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new DeleteAttributeHandler(context);

        var result = await handler.Handle(
            new DeleteAttributeCommand(groupId, attributeId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_NonExistentAttribute_ReturnsNotFound()
    {
        var (groupId, _) = await SeedAttributeAsync();
        await using var context = fixture.CreateContext();
        var handler = new DeleteAttributeHandler(context);

        var result = await handler.Handle(
            new DeleteAttributeCommand(groupId, 999999),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_AttributeWithProductValues_ReturnsBadRequest()
    {
        var (groupId, attributeId) = await SeedAttributeAsync();

        await using var context = fixture.CreateContext();
        var productHandler = new CreateProductHandler(context);
        var product = await productHandler.Handle(
            new CreateProductCommand($"Spec Product {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);

        context.ProductAttributeValues.Add(new ProductAttributeValue
        {
            ProductId = product.Value!.Id,
            AttributeId = attributeId,
            ValueText = "16GB",
            CreatedAt = DateTime.Now
        });
        await context.SaveChangesAsync();

        var handler = new DeleteAttributeHandler(context);
        var result = await handler.Handle(
            new DeleteAttributeCommand(groupId, attributeId),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("specifications", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
