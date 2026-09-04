using backend.Features.BrandFeature.CreateBrand;
using backend.Features.CategoryFeature.CreateCategory;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductFeature.GetProductFilterOptions;
using backend.Features.ProductFeature.ListProductPaginated;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

/// <summary>
/// A parent category normally holds no products directly — everything is filed
/// under its children — so browsing it must reach down the tree.
/// </summary>
[Collection("Database")]
public class CategoryDescendantFilterTests(DatabaseFixture fixture)
{
    private async Task<(long ParentId, long ChildId, long GrandChildId)> SeedCategoryTreeAsync()
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateCategoryHandler(ctx);
        var suffix = Guid.NewGuid().ToString("N");

        var parent = await handler.Handle(
            new CreateCategoryCommand($"Parent {suffix}", null, null, null, null),
            CancellationToken.None);

        var child = await handler.Handle(
            new CreateCategoryCommand($"Child {suffix}", null, null, null, parent.Value!.Id),
            CancellationToken.None);

        var grandChild = await handler.Handle(
            new CreateCategoryCommand($"GrandChild {suffix}", null, null, null, child.Value!.Id),
            CancellationToken.None);

        return (parent.Value.Id, child.Value.Id, grandChild.Value!.Id);
    }

    private async Task<long> SeedProductAsync(long categoryId, string name, long? brandId = null)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateProductHandler(ctx);
        var product = await handler.Handle(
            new CreateProductCommand(name, null, categoryId, brandId, Price: 500, StockQuantity: 3),
            CancellationToken.None);

        return product.Value!.Id;
    }

    [Fact]
    public async Task Handle_ParentCategoryWithoutFlag_ReturnsNoProducts()
    {
        var (parentId, childId, _) = await SeedCategoryTreeAsync();
        await SeedProductAsync(childId, $"Child Product {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(CategoryId: parentId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
    }

    [Fact]
    public async Task Handle_ParentCategoryWithDescendants_ReturnsProductsFromWholeTree()
    {
        var (parentId, childId, grandChildId) = await SeedCategoryTreeAsync();
        var directId = await SeedProductAsync(parentId, $"Direct Product {Guid.NewGuid():N}");
        var childProductId = await SeedProductAsync(childId, $"Child Product {Guid.NewGuid():N}");
        var grandChildProductId = await SeedProductAsync(grandChildId, $"GrandChild Product {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(CategoryId: parentId, IncludeDescendants: true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var ids = result.Value!.Items.Select(i => i.Id).ToList();
        Assert.Contains(directId, ids);
        Assert.Contains(childProductId, ids);
        Assert.Contains(grandChildProductId, ids);
        Assert.Equal(3, result.Value.Total);
    }

    [Fact]
    public async Task Handle_ParentCategoryWithDescendants_ExcludesUnrelatedCategories()
    {
        var (parentId, childId, _) = await SeedCategoryTreeAsync();
        await SeedProductAsync(childId, $"Child Product {Guid.NewGuid():N}");

        var (_, otherChildId, _) = await SeedCategoryTreeAsync();
        var outsiderId = await SeedProductAsync(otherChildId, $"Outsider {Guid.NewGuid():N}");

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);

        var result = await handler.Handle(
            new ListProductPaginatedQuery(CategoryId: parentId, IncludeDescendants: true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(outsiderId, result.Value!.Items.Select(i => i.Id));
    }

    [Fact]
    public async Task Handle_FilterOptionsForParentCategory_IncludesDescendantFacets()
    {
        var (parentId, childId, _) = await SeedCategoryTreeAsync();

        await using var brandCtx = fixture.CreateContext();
        var brand = await new CreateBrandHandler(brandCtx).Handle(
            new CreateBrandCommand($"Descendant Brand {Guid.NewGuid():N}"),
            CancellationToken.None);

        await SeedProductAsync(childId, $"Child Product {Guid.NewGuid():N}", brand.Value!.Id);

        await using var context = fixture.CreateContext();
        var handler = new GetProductFilterOptionsHandler(context);

        var withoutFlag = await handler.Handle(
            new GetProductFilterOptionsQuery(CategoryId: parentId),
            CancellationToken.None);
        var withFlag = await handler.Handle(
            new GetProductFilterOptionsQuery(CategoryId: parentId, IncludeDescendants: true),
            CancellationToken.None);

        Assert.True(withoutFlag.IsSuccess);
        Assert.DoesNotContain(withoutFlag.Value!.Brands, b => b.Id == brand.Value.Id);

        Assert.True(withFlag.IsSuccess);
        Assert.Contains(withFlag.Value!.Brands, b => b.Id == brand.Value.Id);
    }
}
