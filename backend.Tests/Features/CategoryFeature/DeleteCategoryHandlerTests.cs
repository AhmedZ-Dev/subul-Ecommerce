using backend.Domain.Entities;
using backend.Features.CategoryFeature.CreateCategory;
using backend.Features.CategoryFeature.DeleteCategory;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CategoryFeature;

[Collection("Database")]
public class DeleteCategoryHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedCategoryAsync(string nameEn, long? parentId = null)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateCategoryHandler(ctx);
        var result = await handler.Handle(
            new CreateCategoryCommand(nameEn, null, null, null, parentId),
            CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ExistingLeafCategory_DeletesSuccessfully()
    {
        var id = await SeedCategoryAsync("Delete Me");
        await using var context = fixture.CreateContext();
        var handler = new DeleteCategoryHandler(context);

        var result = await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new DeleteCategoryHandler(context);

        var result = await handler.Handle(new DeleteCategoryCommand(999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_CategoryWithSubCategories_ReturnsBadRequest()
    {
        var parentId = await SeedCategoryAsync("Parent With Children Delete");
        await SeedCategoryAsync("Child Of Parent Delete", parentId);

        await using var context = fixture.CreateContext();
        var handler = new DeleteCategoryHandler(context);

        var result = await handler.Handle(new DeleteCategoryCommand(parentId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("subcategor", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_CategoryWithProducts_ReturnsBadRequest()
    {
        var categoryId = await SeedCategoryAsync("Category With Products Delete");

        await using var context = fixture.CreateContext();

        var product = new Product
        {
            NameEn = "Test Product",
            Slug = "test-product-delete-guard",
            CategoryId = categoryId,
            Price = 10,
            Currency = "SAR",
            Status = "active",
            StockQuantity = 5,
            IsFeatured = false,
            RequiresShipping = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new DeleteCategoryHandler(context);
        var result = await handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("products", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
