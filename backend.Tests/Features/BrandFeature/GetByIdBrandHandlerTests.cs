using backend.Features.BrandFeature.CreateBrand;
using backend.Features.BrandFeature.GetByIdBrand;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class GetByIdBrandHandlerTests(DatabaseFixture fixture)
{
    private async Task<long> SeedBrandAsync(string name)
    {
        await using var ctx = fixture.CreateContext();
        var handler = new CreateBrandHandler(ctx);
        var result = await handler.Handle(new CreateBrandCommand(name), CancellationToken.None);
        return result.Value!.Id;
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsBrandWithCorrectData()
    {
        var id = await SeedBrandAsync("GetById Target Brand");
        await using var context = fixture.CreateContext();
        var handler = new GetByIdBrandHandler(context);

        var result = await handler.Handle(new GetByIdBrandQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("GetById Target Brand", result.Value.Name);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdBrandHandler(context);

        var result = await handler.Handle(new GetByIdBrandQuery(999999), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ReturnsCounts_AsZeroForNewBrand()
    {
        var id = await SeedBrandAsync("GetById Counts Test Brand");
        await using var context = fixture.CreateContext();
        var handler = new GetByIdBrandHandler(context);

        var result = await handler.Handle(new GetByIdBrandQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!._count.Product);
    }
}
