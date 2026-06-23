using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.CollectionFeature.GetByIdCollection;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.CollectionFeature;

[Collection("Database")]
public class GetByIdCollectionHandlerTests(DatabaseFixture fixture)
{
    private async Task<Product> CreateTestProductAsync(AppDbContext context, string name)
    {
        var product = new Product
        {
            NameEn = name,
            Slug = name.ToLower().Replace(" ", "-"),
            Status = "active",
            Price = 99.99m,
            Currency = "IQD",
            CreatedAt = DateTime.Now
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsCollection()
    {
        await using var context = fixture.CreateContext();
        var p1 = await CreateTestProductAsync(context, "Prod A " + Guid.NewGuid());

        var collection = new Collection
        {
            NameEn = "Test Collection " + Guid.NewGuid(),
            Slug = "test-collection-" + Guid.NewGuid().ToString("N"),
            CollectionType = "manual",
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        collection.CollectionProducts.Add(new CollectionProduct
        {
            ProductId = p1.Id,
            SortOrder = 5,
            CreatedAt = DateTime.Now
        });

        context.Collections.Add(collection);
        await context.SaveChangesAsync();

        var handler = new GetByIdCollectionHandler(context);
        var query = new GetByIdCollectionQuery(collection.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(collection.NameEn, result.Value.NameEn);
        Assert.Single(result.Value.Products);
        Assert.Equal(p1.Id, result.Value.Products[0].ProductId);
        Assert.Equal(5, result.Value.Products[0].SortOrder);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdCollectionHandler(context);
        var query = new GetByIdCollectionQuery(999999);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
