using backend.Domain.Entities;
using backend.Features.CategoryFeature.CreateCategory;
using backend.Features.CategoryFeature.UpdateCategory;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CategoryFeature;

[Collection("Database")]
public class UpdateCategoryHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var id = await SeedCategoryAsync("Update Target");
        await using var context = fixture.CreateContext();
        var handler = new UpdateCategoryHandler(context);
        var command = new UpdateCategoryCommand(
            Id: id,
            NameEn: "Updated Name",
            NameAr: "اسم محدث",
            DescriptionEn: "New desc",
            DescriptionAr: null,
            ParentId: null,
            Slug: null,
            ImageUrl: null,
            BannerUrl: null,
            SortOrder: 5,
            IsActive: false,
            SeoTitle: null,
            SeoDescription: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Value!.NameEn);
        Assert.Equal("اسم محدث", result.Value.NameAr);
        Assert.False(result.Value.IsActive);
        Assert.Equal(5, result.Value.SortOrder);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new UpdateCategoryHandler(context);
        var command = new UpdateCategoryCommand(
            Id: 999999,
            NameEn: "Ghost",
            NameAr: null, DescriptionEn: null, DescriptionAr: null,
            ParentId: null, Slug: null, ImageUrl: null, BannerUrl: null,
            SortOrder: 0, IsActive: true, SeoTitle: null, SeoDescription: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsConflict()
    {
        var idA = await SeedCategoryAsync("Existing Update Cat A");
        var idB = await SeedCategoryAsync("Existing Update Cat B");

        await using var context = fixture.CreateContext();
        var handler = new UpdateCategoryHandler(context);
        var command = new UpdateCategoryCommand(
            Id: idB,
            NameEn: "Existing Update Cat A",
            NameAr: null, DescriptionEn: null, DescriptionAr: null,
            ParentId: null, Slug: null, ImageUrl: null, BannerUrl: null,
            SortOrder: 0, IsActive: true, SeoTitle: null, SeoDescription: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SelfParent_ReturnsBadRequest()
    {
        var id = await SeedCategoryAsync("Self Parent Test");
        await using var context = fixture.CreateContext();
        var handler = new UpdateCategoryHandler(context);
        var command = new UpdateCategoryCommand(
            Id: id,
            NameEn: "Self Parent Test",
            NameAr: null, DescriptionEn: null, DescriptionAr: null,
            ParentId: id,
            Slug: null, ImageUrl: null, BannerUrl: null,
            SortOrder: 0, IsActive: true, SeoTitle: null, SeoDescription: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("own parent", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_NonExistentParentId_ReturnsNotFound()
    {
        var id = await SeedCategoryAsync("Orphan Update Test");
        await using var context = fixture.CreateContext();
        var handler = new UpdateCategoryHandler(context);
        var command = new UpdateCategoryCommand(
            Id: id,
            NameEn: "Orphan Update Test",
            NameAr: null, DescriptionEn: null, DescriptionAr: null,
            ParentId: 888888,
            Slug: null, ImageUrl: null, BannerUrl: null,
            SortOrder: 0, IsActive: true, SeoTitle: null, SeoDescription: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SameNameForSameCategory_DoesNotConflict()
    {
        var id = await SeedCategoryAsync("Same Name No Conflict");
        await using var context = fixture.CreateContext();
        var handler = new UpdateCategoryHandler(context);
        var command = new UpdateCategoryCommand(
            Id: id,
            NameEn: "Same Name No Conflict",
            NameAr: "عربي",
            DescriptionEn: "Updated desc",
            DescriptionAr: null,
            ParentId: null, Slug: null, ImageUrl: null, BannerUrl: null,
            SortOrder: 1, IsActive: true, SeoTitle: null, SeoDescription: null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
