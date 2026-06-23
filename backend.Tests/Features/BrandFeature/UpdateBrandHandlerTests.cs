using backend.Features.BrandFeature.CreateBrand;
using backend.Features.BrandFeature.UpdateBrand;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class UpdateBrandHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedBrandAsync(string name)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateBrandHandler(ctx);
        var result = await handler.Handle(new CreateBrandCommand(name), CancellationToken.None);
        return result.Value!.Id;
    }

    private static UpdateBrandCommand BuildUpdateCommand(long id, string name) =>
        new(
            Id: id,
            Name: name,
            Slug: null,
            LogoUrl: null,
            BannerUrl: null,
            DescriptionEn: "Updated description",
            DescriptionAr: null,
            WebsiteUrl: "https://updated.com",
            IsFeatured: true,
            IsActive: false,
            SortOrder: 3);

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var id = await SeedBrandAsync("Update Target Brand");
        await using var context = fixture.CreateContext();
        var handler = new UpdateBrandHandler(context);
        var command = BuildUpdateCommand(id, "Updated Brand Name");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Brand Name", result.Value!.Name);
        Assert.False(result.Value.IsActive);
        Assert.True(result.Value.IsFeatured);
        Assert.Equal(3, result.Value.SortOrder);
    }

    [Fact]
    public async Task Handle_BrandNotFound_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new UpdateBrandHandler(context);
        var command = BuildUpdateCommand(999999, "Ghost Brand");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_SameNameForSameBrand_UpdatesSuccessfully()
    {
        var id = await SeedBrandAsync("Same Name No Conflict Brand");
        await using var context = fixture.CreateContext();
        var handler = new UpdateBrandHandler(context);
        var command = BuildUpdateCommand(id, "Same Name No Conflict Brand");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
