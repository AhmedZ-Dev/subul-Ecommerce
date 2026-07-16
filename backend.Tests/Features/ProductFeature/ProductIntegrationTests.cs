using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductFeature;

[Collection("Database")]
public class ProductIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ProductIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        await using var context = _fixture.CreateContext();
        _client = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task POST_Products_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            nameEn = $"Integration Product {Guid.NewGuid():N}",
            nameAr = "منتج تكامل",
            categoryId = (long?)null,
            brandId = (long?)null,
            price = 1500m
        };

        var response = await _client.PostAsJsonAsync("/api/products", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_Products_DuplicateSku_Returns409()
    {
        var sku = $"INT-SKU-{Guid.NewGuid():N}";
        var payloadA = new { nameEn = $"Product A {Guid.NewGuid():N}", categoryId = (long?)null, brandId = (long?)null, sku, price = 100m };
        var payloadB = new { nameEn = $"Product B {Guid.NewGuid():N}", categoryId = (long?)null, brandId = (long?)null, sku, price = 200m };

        await _client.PostAsJsonAsync("/api/products", payloadA);
        var response = await _client.PostAsJsonAsync("/api/products", payloadB);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task POST_Products_MissingRequiredField_Returns400()
    {
        var payload = new { nameAr = "بدون انجليزي", price = 100m };

        var response = await _client.PostAsJsonAsync("/api/products", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GET_Products_ById_ExistingId_Returns200WithProduct()
    {
        var created = await CreateProductAsync($"GetById Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/products/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
        Assert.True(body.GetProperty("data").TryGetProperty("variants", out var variants));
        Assert.Equal(JsonValueKind.Array, variants.ValueKind);
        Assert.True(body.GetProperty("data").TryGetProperty("attributeValues", out var attributeValues));
        Assert.Equal(JsonValueKind.Array, attributeValues.ValueKind);
    }

    [Fact]
    public async Task GET_Products_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/products/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Products_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/products?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
        Assert.True(data.TryGetProperty("page", out _));
        Assert.True(data.TryGetProperty("limit", out _));
        Assert.True(data.TryGetProperty("totalPages", out _));
    }

    [Fact]
    public async Task GET_Products_FilterOptions_Returns200WithFacets()
    {
        var response = await _client.GetAsync("/api/products/filter-options");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("brands", out var brands));
        Assert.True(data.TryGetProperty("priceRange", out var priceRange));
        Assert.True(data.TryGetProperty("attributeGroups", out var attributeGroups));
        Assert.Equal(JsonValueKind.Array, brands.ValueKind);
        Assert.True(priceRange.TryGetProperty("min", out _));
        Assert.True(priceRange.TryGetProperty("max", out _));
        Assert.Equal(JsonValueKind.Array, attributeGroups.ValueKind);
    }

    [Fact]
    public async Task GET_Products_List_WithAdvancedFilters_Returns200()
    {
        var response = await _client.GetAsync(
            "/api/products?status=active&minPrice=100&maxPrice=5000&inStockOnly=true&page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task PUT_Products_ValidUpdate_Returns200()
    {
        var created = await CreateProductAsync($"Update Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedName = $"Updated Integration {Guid.NewGuid():N}";

        var payload = BuildUpdatePayload(updatedName);

        var response = await _client.PutAsJsonAsync($"/api/products/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedName, body.GetProperty("data").GetProperty("nameEn").GetString());
    }

    [Fact]
    public async Task PUT_Products_NonExistentId_Returns404()
    {
        var payload = BuildUpdatePayload("Ghost Product");

        var response = await _client.PutAsJsonAsync("/api/products/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Products_ExistingProduct_Returns200()
    {
        var created = await CreateProductAsync($"Delete Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/products/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Products_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/products/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateProductAsync(string nameEn)
    {
        var payload = new { nameEn, nameAr = (string?)null, categoryId = (long?)null, brandId = (long?)null, price = 100m };
        var response = await _client.PostAsJsonAsync("/api/products", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static object BuildUpdatePayload(string nameEn) => new
    {
        nameEn,
        nameAr = (string?)null,
        categoryId = (long?)null,
        brandId = (long?)null,
        slug = (string?)null,
        sku = (string?)null,
        barcode = (string?)null,
        descriptionEn = (string?)null,
        descriptionAr = (string?)null,
        shortDescriptionEn = (string?)null,
        shortDescriptionAr = (string?)null,
        price = 200m,
        compareAtPrice = (decimal?)null,
        costPrice = (decimal?)null,
        currency = "IQD",
        stockQuantity = 10,
        lowStockThreshold = 2,
        minOrderQuantity = 1,
        weight = (decimal?)null,
        status = "active",
        isFeatured = false,
        requiresShipping = true,
        warrantyMonths = 12,
        warrantyDescription = (string?)null,
        metaTitle = (string?)null,
        metaDescription = (string?)null
    };

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
