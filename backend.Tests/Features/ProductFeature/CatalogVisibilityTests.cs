using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Domain.Entities;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

/// <summary>
/// Regression tests for the Wave B finding M-9.
///
/// The catalog reads are [AllowAnonymous] so the storefront can call them, which
/// also meant an anonymous caller could set any filter the admin panel can:
/// <c>?status=draft</c> enumerated the unreleased catalogue and <c>?isActive=false</c>
/// the disabled rows. The endpoints now pin the visibility filter for callers
/// without a token, and the get-by-id/slug routes answer 404 for hidden rows.
///
/// These assert at the HTTP level on purpose — the guard lives in the controller,
/// so a handler-level test would pass even with the fix reverted.
/// </summary>
[Collection("Database")]
public class CatalogVisibilityTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _anonymousClient = null!;
    private HttpClient _adminClient = null!;

    private long _draftProductId;
    private string _draftProductSlug = null!;
    private long _inactiveBrandId;
    private long _inactiveCategoryId;
    private string _inactiveCategorySlug = null!;
    private long _inactiveCollectionId;
    private long _inactiveZoneId;

    public CatalogVisibilityTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _anonymousClient = _factory.CreateClient();

        await using var context = _fixture.CreateContext();
        _adminClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);

        var tag = Guid.NewGuid().ToString("N");
        var now = DateTime.Now;

        var product = new Product
        {
            NameEn = $"Unreleased Product {tag}",
            Slug = $"unreleased-product-{tag}",
            Price = 100,
            Currency = "IQD",
            Status = "draft",
            StockQuantity = 5,
            CreatedAt = now,
        };
        var brand = new Brand
        {
            Name = $"Hidden Brand {tag}",
            Slug = $"hidden-brand-{tag}",
            IsActive = false,
            CreatedAt = now,
        };
        var category = new Category
        {
            NameEn = $"Hidden Category {tag}",
            Slug = $"hidden-category-{tag}",
            IsActive = false,
            CreatedAt = now,
        };
        var collection = new Collection
        {
            NameEn = $"Hidden Collection {tag}",
            Slug = $"hidden-collection-{tag}",
            CollectionType = "manual",
            IsActive = false,
            CreatedAt = now,
        };
        var zone = new ShippingZone
        {
            NameEn = $"Hidden Zone {tag}",
            Governorates = "[\"Baghdad\"]",
            IsActive = false,
            CreatedAt = now,
        };

        context.Products.Add(product);
        context.Brands.Add(brand);
        context.Categories.Add(category);
        context.Collections.Add(collection);
        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync();

        _draftProductId = product.Id;
        _draftProductSlug = product.Slug;
        _inactiveBrandId = brand.Id;
        _inactiveCategoryId = category.Id;
        _inactiveCategorySlug = category.Slug;
        _inactiveCollectionId = collection.Id;
        _inactiveZoneId = zone.Id;
    }

    public async Task DisposeAsync()
    {
        _anonymousClient.Dispose();
        _adminClient.Dispose();
        await _factory.DisposeAsync();
    }

    private static async Task<List<long>> ItemIdsAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("data").GetProperty("items")
            .EnumerateArray()
            .Select(x => x.GetProperty("id").GetInt64())
            .ToList();
    }

    // The headline exploit: ?status=draft against the public catalog.
    [Fact]
    public async Task GET_Products_AnonymousWithDraftFilter_DoesNotLeakUnpublished()
    {
        var response = await _anonymousClient.GetAsync("/api/products?status=draft&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(_draftProductId, await ItemIdsAsync(response));
    }

    [Fact]
    public async Task GET_Products_AdminWithDraftFilter_StillSeesUnpublished()
    {
        var response = await _adminClient.GetAsync("/api/products?status=draft&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(_draftProductId, await ItemIdsAsync(response));
    }

    [Fact]
    public async Task GET_ProductById_AnonymousDraft_Returns404()
    {
        var response = await _anonymousClient.GetAsync($"/api/products/{_draftProductId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_ProductBySlug_AnonymousDraft_Returns404()
    {
        var response = await _anonymousClient.GetAsync($"/api/products/by-slug/{_draftProductSlug}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_ProductById_AdminDraft_Returns200()
    {
        var response = await _adminClient.GetAsync($"/api/products/{_draftProductId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Brands_AnonymousWithInactiveFilter_DoesNotLeakDisabled()
    {
        var response = await _anonymousClient.GetAsync("/api/brands?isActive=false&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(_inactiveBrandId, await ItemIdsAsync(response));
    }

    [Fact]
    public async Task GET_BrandById_AnonymousInactive_Returns404()
    {
        var response = await _anonymousClient.GetAsync($"/api/brands/{_inactiveBrandId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_Categories_AnonymousWithInactiveFilter_DoesNotLeakDisabled()
    {
        var response = await _anonymousClient.GetAsync("/api/categories?isActive=false&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(_inactiveCategoryId, await ItemIdsAsync(response));
    }

    [Fact]
    public async Task GET_CategoryBySlug_AnonymousInactive_Returns404()
    {
        var response = await _anonymousClient.GetAsync($"/api/categories/by-slug/{_inactiveCategorySlug}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_Collections_AnonymousWithInactiveFilter_DoesNotLeakDisabled()
    {
        var response = await _anonymousClient.GetAsync("/api/collections?isActive=false&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(_inactiveCollectionId, await ItemIdsAsync(response));
    }

    [Fact]
    public async Task GET_CollectionById_AnonymousInactive_Returns404()
    {
        var response = await _anonymousClient.GetAsync($"/api/collections/{_inactiveCollectionId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_ShippingZones_AnonymousWithInactiveFilter_DoesNotLeakDisabled()
    {
        var response = await _anonymousClient.GetAsync("/api/shipping-zones?isActive=false&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(_inactiveZoneId, await ItemIdsAsync(response));
    }

    [Fact]
    public async Task GET_ShippingZoneById_AnonymousInactive_Returns404()
    {
        var response = await _anonymousClient.GetAsync($"/api/shipping-zones/{_inactiveZoneId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // The admin panel must keep seeing everything — the fix scopes visibility,
    // it does not remove the ability to manage hidden rows.
    [Fact]
    public async Task GET_Brands_AdminWithInactiveFilter_StillSeesDisabled()
    {
        var response = await _adminClient.GetAsync("/api/brands?isActive=false&limit=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(_inactiveBrandId, await ItemIdsAsync(response));
    }
}
