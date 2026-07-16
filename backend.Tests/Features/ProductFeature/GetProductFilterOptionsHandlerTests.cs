using backend.Domain.Entities;
using backend.Features.AttributeGroupFeature.CreateAttributeGroup;
using backend.Features.BrandFeature.CreateBrand;
using backend.Features.ProductAttributeValueFeature.CreateProductAttributeValue;
using backend.Features.ProductFeature.CreateProduct;
using backend.Features.ProductFeature.GetProductFilterOptions;
using backend.Features.ProductFeature.ListProductPaginated;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

[Collection("Database")]
public class GetProductFilterOptionsHandlerTests(DatabaseFixture fixture)
{
    private async Task<(long ProductId, long BrandId, long FilterableGroupId, long NonFilterableGroupId)> SeedFilterDataAsync()
    {
        await using var ctx = fixture.CreateContext();

        var brandHandler = new CreateBrandHandler(ctx);
        var brand = await brandHandler.Handle(
            new CreateBrandCommand($"Filter Brand {Guid.NewGuid():N}"),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(ctx);
        var product = await productHandler.Handle(
            new CreateProductCommand(
                $"Filter Product {Guid.NewGuid():N}",
                null,
                null,
                brand.Value!.Id,
                Price: 1500,
                StockQuantity: 5),
            CancellationToken.None);

        var filterableGroupHandler = new CreateAttributeGroupHandler(ctx);
        var filterableGroup = await filterableGroupHandler.Handle(
            new CreateAttributeGroupCommand(
                $"Filterable Group {Guid.NewGuid():N}",
                IsFilterable: true,
                Attributes:
                [
                    new CreateAttributeGroupAttributeInput("RAM", InputType: "text", IsFilterable: true)
                ]),
            CancellationToken.None);

        var nonFilterableGroupHandler = new CreateAttributeGroupHandler(ctx);
        var nonFilterableGroup = await nonFilterableGroupHandler.Handle(
            new CreateAttributeGroupCommand(
                $"Hidden Group {Guid.NewGuid():N}",
                IsFilterable: false,
                Attributes:
                [
                    new CreateAttributeGroupAttributeInput("Hidden", InputType: "text", IsFilterable: false)
                ]),
            CancellationToken.None);

        var ramAttributeId = filterableGroup.Value!.Attributes[0].Id;
        var hiddenAttributeId = nonFilterableGroup.Value!.Attributes[0].Id;

        var valueHandler = new CreateProductAttributeValueHandler(ctx);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value!.Id, ramAttributeId, ValueText: "16GB"),
            CancellationToken.None);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(product.Value.Id, hiddenAttributeId, ValueText: "secret"),
            CancellationToken.None);

        return (product.Value.Id, brand.Value.Id, filterableGroup.Value.Id, nonFilterableGroup.Value.Id);
    }

    [Fact]
    public async Task Handle_ReturnsBrandFacetsWithCounts()
    {
        var (_, brandId, _, _) = await SeedFilterDataAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetProductFilterOptionsHandler(context);

        var result = await handler.Handle(new GetProductFilterOptionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.Brands, b => b.Id == brandId && b.Count >= 1);
    }

    [Fact]
    public async Task Handle_ReturnsPriceRange()
    {
        await SeedFilterDataAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetProductFilterOptionsHandler(context);

        var result = await handler.Handle(new GetProductFilterOptionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.PriceRange.Min >= 0);
        Assert.True(result.Value.PriceRange.Max >= result.Value.PriceRange.Min);
    }

    [Fact]
    public async Task Handle_ReturnsOnlyFilterableAttributeGroups()
    {
        var (_, _, filterableGroupId, nonFilterableGroupId) = await SeedFilterDataAsync();
        await using var context = fixture.CreateContext();
        var handler = new GetProductFilterOptionsHandler(context);

        var result = await handler.Handle(new GetProductFilterOptionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.AttributeGroups, g => g.Id == filterableGroupId);
        Assert.DoesNotContain(result.Value.AttributeGroups, g => g.Id == nonFilterableGroupId);
        var group = result.Value.AttributeGroups.Single(g => g.Id == filterableGroupId);
        Assert.Contains(group.Values, v => v.Value == "16GB" && v.Count >= 1);
    }

    [Fact]
    public async Task Handle_ScopesToCategoryId()
    {
        await using var ctx = fixture.CreateContext();
        var category = new Category
        {
            NameEn = $"Filter Category {Guid.NewGuid():N}",
            Slug = $"filter-category-{Guid.NewGuid():N}",
            SortOrder = 0,
            IsActive = true,
            CreatedAt = DateTime.Now,
        };
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();

        var productHandler = new CreateProductHandler(ctx);
        await productHandler.Handle(
            new CreateProductCommand(
                $"Scoped Product {Guid.NewGuid():N}",
                null,
                category.Id,
                null,
                Price: 900),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new GetProductFilterOptionsHandler(context);
        var result = await handler.Handle(
            new GetProductFilterOptionsQuery(CategoryId: category.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.PriceRange.Max >= 900);
    }
}

[Collection("Database")]
public class ListProductAdvancedFilterHandlerTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task Handle_FilterByPriceRange_ReturnsMatchingProducts()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        await productHandler.Handle(
            new CreateProductCommand($"Cheap {Guid.NewGuid():N}", null, null, null, Price: 100),
            CancellationToken.None);
        await productHandler.Handle(
            new CreateProductCommand($"Expensive {Guid.NewGuid():N}", null, null, null, Price: 5000),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductPaginatedQuery(MinPrice: 200, MaxPrice: 1000, Status: "active"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.All(result.Value!.Items, item =>
        {
            Assert.True(item.Price >= 200);
            Assert.True(item.Price <= 1000);
        });
    }

    [Fact]
    public async Task Handle_FilterInStockOnly_ExcludesZeroStock()
    {
        await using var ctx = fixture.CreateContext();
        var productHandler = new CreateProductHandler(ctx);
        var inStock = await productHandler.Handle(
            new CreateProductCommand($"In Stock {Guid.NewGuid():N}", null, null, null, StockQuantity: 3),
            CancellationToken.None);
        await productHandler.Handle(
            new CreateProductCommand($"Out Stock {Guid.NewGuid():N}", null, null, null, StockQuantity: 0),
            CancellationToken.None);

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductPaginatedQuery(
                InStockOnly: true,
                Search: inStock.Value!.NameEn,
                Status: "active"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.Items, item => item.Id == inStock.Value.Id);
        Assert.All(result.Value.Items, item => Assert.True(item.StockQuantity > 0));
    }

    [Fact]
    public async Task Handle_FilterByAttributeGroup_ReturnsMatchingProducts()
    {
        await using var ctx = fixture.CreateContext();
        var groupHandler = new CreateAttributeGroupHandler(ctx);
        var group = await groupHandler.Handle(
            new CreateAttributeGroupCommand(
                $"List Attr Group {Guid.NewGuid():N}",
                IsFilterable: true,
                Attributes: [new CreateAttributeGroupAttributeInput("RAM", InputType: "text")]),
            CancellationToken.None);

        var productHandler = new CreateProductHandler(ctx);
        var matching = await productHandler.Handle(
            new CreateProductCommand($"Attr Match {Guid.NewGuid():N}", null, null, null),
            CancellationToken.None);
        var other = await productHandler.Handle(
            new CreateProductCommand($"Attr Other {Guid.NewGuid():N}", null, null, null),
            CancellationToken.None);

        var valueHandler = new CreateProductAttributeValueHandler(ctx);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(
                matching.Value!.Id,
                group.Value!.Attributes[0].Id,
                ValueText: "32GB"),
            CancellationToken.None);
        await valueHandler.Handle(
            new CreateProductAttributeValueCommand(
                other.Value!.Id,
                group.Value.Attributes[0].Id,
                ValueText: "8GB"),
            CancellationToken.None);

        var attrs = $"{{\"{group.Value.Id}\":[\"32GB\"]}}";

        await using var context = fixture.CreateContext();
        var handler = new ListProductPaginatedHandler(context);
        var result = await handler.Handle(
            new ListProductPaginatedQuery(Attrs: attrs, Status: "active"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Value!.Items, item => item.Id == matching.Value.Id);
        Assert.DoesNotContain(result.Value.Items, item => item.Id == other.Value.Id);
    }
}
