using backend.Features.BrandFeature.CreateBrand;
using backend.Features.BrandFeature.DeleteBrandLogo;
using backend.Features.BrandFeature.UploadBrandLogo;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class UploadBrandLogoHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedBrandAsync(string name)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateBrandHandler(ctx);
        var result = await handler.Handle(new CreateBrandCommand(name), CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ValidFile_SavesLogoUrl()
    {
        var brandId = await SeedBrandAsync($"Logo Upload {Guid.NewGuid():N}");
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new UploadBrandLogoHandler(context, storage);

        var result = await handler.Handle(
            new UploadBrandLogoCommand(brandId, ProductImageTestHelpers.CreatePngFormFile()),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.StartsWith($"/img/brands/{brandId}/logo-", result.Value!.LogoUrl);
    }

    [Fact]
    public async Task Handle_NonExistentBrand_ReturnsNotFound()
    {
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new UploadBrandLogoHandler(context, storage);

        var result = await handler.Handle(
            new UploadBrandLogoCommand(999999, ProductImageTestHelpers.CreatePngFormFile()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_InvalidFile_ReturnsFailure()
    {
        var brandId = await SeedBrandAsync($"Logo Invalid {Guid.NewGuid():N}");
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new UploadBrandLogoHandler(context, storage);

        var result = await handler.Handle(
            new UploadBrandLogoCommand(brandId, ProductImageTestHelpers.CreateInvalidFormFile()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_FakePngExtension_ReturnsFailure()
    {
        var brandId = await SeedBrandAsync($"Logo Fake Png {Guid.NewGuid():N}");
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new UploadBrandLogoHandler(context, storage);

        var result = await handler.Handle(
            new UploadBrandLogoCommand(brandId, ProductImageTestHelpers.CreateFakePngFormFile()),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid image file", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_MissingFile_ReturnsFailure()
    {
        var brandId = await SeedBrandAsync($"Logo Missing {Guid.NewGuid():N}");
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new UploadBrandLogoHandler(context, storage);

        var result = await handler.Handle(
            new UploadBrandLogoCommand(brandId, null!),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ReplaceLogo_DeletesPreviousFileFromDisk()
    {
        var brandId = await SeedBrandAsync($"Logo Replace {Guid.NewGuid():N}");
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var handler = new UploadBrandLogoHandler(context, storage);

        var firstResult = await handler.Handle(
            new UploadBrandLogoCommand(brandId, ProductImageTestHelpers.CreatePngFormFile()),
            CancellationToken.None);
        Assert.True(firstResult.IsSuccess);

        var firstPhysicalPath = storage.GetPhysicalPathForRelativeUrl(firstResult.Value!.LogoUrl!);
        Assert.True(File.Exists(firstPhysicalPath));

        var secondResult = await handler.Handle(
            new UploadBrandLogoCommand(brandId, ProductImageTestHelpers.CreatePngFormFile()),
            CancellationToken.None);

        Assert.True(secondResult.IsSuccess);
        Assert.False(File.Exists(firstPhysicalPath));
        Assert.True(File.Exists(storage.GetPhysicalPathForRelativeUrl(secondResult.Value!.LogoUrl!)));
    }

    [Fact]
    public async Task DeleteLogo_ExistingLogo_ClearsUrl()
    {
        var brandId = await SeedBrandAsync($"Logo Delete {Guid.NewGuid():N}");
        var storage = ProductImageTestHelpers.CreateStorageService();
        await using var context = fixture.CreateContext();
        var uploadHandler = new UploadBrandLogoHandler(context, storage);
        await uploadHandler.Handle(
            new UploadBrandLogoCommand(brandId, ProductImageTestHelpers.CreatePngFormFile()),
            CancellationToken.None);

        var deleteHandler = new DeleteBrandLogoHandler(context, storage);
        var result = await deleteHandler.Handle(
            new DeleteBrandLogoCommand(brandId),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.LogoUrl);
    }
}
