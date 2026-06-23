using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.CollectionFeature.UpdateCollection;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.CollectionFeature;

[Collection("Database")]
public class UpdateCollectionHandlerTests(DatabaseFixture fixture)
{
    private async Task<Product> CreateTestProductAsync(AppDbContext context, string name)
    {
        var product = new Product
        {
            NameEn = name,
            Slug = name.ToLower().Replace(" ", "-").Replace("&", "and") + "-" + Guid.NewGuid().ToString("N"),
            Status = "active",
            Price = 99.99m,
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    private async Task<Collection> CreateCollectionAsync(AppDbContext context, string name)
    {
        var collection = new Collection
        {
            NameEn = name,
            Slug = name.ToLower().Replace(" ", "-") + "-" + Guid.NewGuid().ToString("N"),
            CollectionType = "manual",
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        context.Collections.Add(collection);
        await context.SaveChangesAsync();
        return collection;
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesFieldsAndAssociations()
    {
        await using var context = fixture.CreateContext();
        var collection = await CreateCollectionAsync(context, "Old Collection " + Guid.NewGuid());
        var p1 = await CreateTestProductAsync(context, "Prod 1 " + Guid.NewGuid());
        var p2 = await CreateTestProductAsync(context, "Prod 2 " + Guid.NewGuid());

        var handler = new UpdateCollectionHandler(context);
        var command = new UpdateCollectionCommand(
            Id: collection.Id,
            NameEn: "New Title " + Guid.NewGuid(),
            NameAr: "العنوان الجديد",
            DescriptionEn: "Updated Desc",
            DescriptionAr: null,
            ImageUrl: null,
            BannerUrl: null,
            CollectionType: "smart",
            IsActive: false,
            SortOrder: 15,
            MetaTitle: "SEO",
            MetaDescription: "SEO desc",
            Products: new List<CollectionProductInput>
            {
                new(p1.Id, 1),
                new(p2.Id, 2)
            },
            Slug: "new-custom-slug"
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(command.NameEn, result.Value.NameEn);
        Assert.Equal("العنوان الجديد", result.Value.NameAr);
        Assert.Equal("smart", result.Value.CollectionType);
        Assert.False(result.Value.IsActive);
        Assert.Equal(15, result.Value.SortOrder);
        Assert.Equal("new-custom-slug", result.Value.Slug);
        Assert.Equal(2, result.Value.Products.Count);
    }

    [Fact]
    public async Task Handle_NonExistentCollection_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new UpdateCollectionHandler(context);
        var command = new UpdateCollectionCommand(
            Id: 999999,
            NameEn: "Non Existent",
            NameAr: null,
            DescriptionEn: null,
            DescriptionAr: null,
            ImageUrl: null,
            BannerUrl: null,
            CollectionType: "manual",
            IsActive: true,
            SortOrder: 0,
            MetaTitle: null,
            MetaDescription: null,
            Products: null,
            Slug: null
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsConflict()
    {
        await using var context = fixture.CreateContext();
        var col1 = await CreateCollectionAsync(context, "Col Unique 1 " + Guid.NewGuid());
        var col2 = await CreateCollectionAsync(context, "Col Unique 2 " + Guid.NewGuid());

        var handler = new UpdateCollectionHandler(context);
        var command = new UpdateCollectionCommand(
            Id: col2.Id,
            NameEn: col1.NameEn,
            NameAr: null,
            DescriptionEn: null,
            DescriptionAr: null,
            ImageUrl: null,
            BannerUrl: null,
            CollectionType: "manual",
            IsActive: true,
            SortOrder: 0,
            MetaTitle: null,
            MetaDescription: null,
            Products: null,
            Slug: null
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_EmptyProductsList_ClearsAssociations()
    {
        await using var context = fixture.CreateContext();
        var collection = await CreateCollectionAsync(context, "Col With Products " + Guid.NewGuid());
        var p1 = await CreateTestProductAsync(context, "Prod 1 " + Guid.NewGuid());
        collection.CollectionProducts.Add(new CollectionProduct { ProductId = p1.Id, SortOrder = 1, CreatedAt = DateTime.Now });
        await context.SaveChangesAsync();

        var handler = new UpdateCollectionHandler(context);
        var command = new UpdateCollectionCommand(
            Id: collection.Id,
            NameEn: collection.NameEn,
            NameAr: null,
            DescriptionEn: null,
            DescriptionAr: null,
            ImageUrl: null,
            BannerUrl: null,
            CollectionType: "manual",
            IsActive: true,
            SortOrder: 0,
            MetaTitle: null,
            MetaDescription: null,
            Products: new List<CollectionProductInput>(), // Empty list
            Slug: null
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Products);
    }
}
