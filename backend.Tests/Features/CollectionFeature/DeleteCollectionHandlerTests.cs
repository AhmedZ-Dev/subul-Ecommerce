using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.CollectionFeature.DeleteCollection;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests.Features.CollectionFeature;

[Collection("Database")]
public class DeleteCollectionHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ExistingId_DeletesCollectionAndAssociations()
    {
        await using var context = fixture.CreateContext();
        var p1 = await CreateTestProductAsync(context, "Prod for Del " + Guid.NewGuid());

        var collection = new Collection
        {
            NameEn = "Delete Me Col " + Guid.NewGuid(),
            Slug = "delete-me-" + Guid.NewGuid().ToString("N"),
            CollectionType = "manual",
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        collection.CollectionProducts.Add(new CollectionProduct
        {
            ProductId = p1.Id,
            SortOrder = 1,
            CreatedAt = DateTime.Now
        });

        context.Collections.Add(collection);
        await context.SaveChangesAsync();

        var handler = new DeleteCollectionHandler(context);
        var command = new DeleteCollectionCommand(collection.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);

        var deletedCollection = await context.Collections.AnyAsync(c => c.Id == collection.Id);
        Assert.False(deletedCollection);

        var deletedAssociations = await context.CollectionProducts.AnyAsync(cp => cp.CollectionId == collection.Id);
        Assert.False(deletedAssociations);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new DeleteCollectionHandler(context);
        var command = new DeleteCollectionCommand(999999);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
