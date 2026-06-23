using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductVariantFeature;

[Collection("Database")]
public class ProductVariantIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public ProductVariantIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task POST_Variants_ValidPayload_Returns201WithData()
    {
        var productId = await CreateProductAsync();
        var payload = new
        {
            title = "16GB / 512GB SSD",
            sku = $"INT-VAR-{Guid.NewGuid():N}",
            price = 1625000m,
            stockQuantity = 10
        };

        var response = await _client.PostAsJsonAsync($"/api/products/{productId}/variants", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(productId, body.GetProperty("data").GetProperty("productId").GetInt64());
    }

    [Fact]
    public async Task POST_Variants_NonExistentProduct_Returns404()
    {
        var payload = new { title = "Orphan Variant", stockQuantity = 0 };

        var response = await _client.PostAsJsonAsync("/api/products/999999999/variants", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Variants_List_Returns200WithPaginatedShape()
    {
        var productId = await CreateProductAsync();
        await _client.PostAsJsonAsync($"/api/products/{productId}/variants", new { title = "List Variant", stockQuantity = 1 });

        var response = await _client.GetAsync($"/api/products/{productId}/variants?page=1&limit=5");
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
    public async Task GET_Variants_ById_ExistingId_Returns200()
    {
        var productId = await CreateProductAsync();
        var created = await CreateVariantAsync(productId, "GetById Variant");
        var variantId = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/products/{productId}/variants/{variantId}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(variantId, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_Variants_ById_NonExistentId_Returns404()
    {
        var productId = await CreateProductAsync();

        var response = await _client.GetAsync($"/api/products/{productId}/variants/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task PUT_Variants_ValidUpdate_Returns200()
    {
        var productId = await CreateProductAsync();
        var created = await CreateVariantAsync(productId, "Before Update");
        var variantId = created.GetProperty("data").GetProperty("id").GetInt64();

        var payload = new
        {
            title = "After Update",
            sku = (string?)null,
            barcode = (string?)null,
            price = 2000m,
            compareAtPrice = (decimal?)null,
            costPrice = (decimal?)null,
            stockQuantity = 20,
            weight = (decimal?)null,
            isActive = true,
            sortOrder = 1
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{productId}/variants/{variantId}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("After Update", body.GetProperty("data").GetProperty("title").GetString());
    }

    [Fact]
    public async Task DELETE_Variants_ExistingVariant_Returns200()
    {
        var productId = await CreateProductAsync();
        var created = await CreateVariantAsync(productId, "Delete Variant");
        var variantId = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/products/{productId}/variants/{variantId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Variants_NonExistentId_Returns404()
    {
        var productId = await CreateProductAsync();

        var response = await _client.DeleteAsync($"/api/products/{productId}/variants/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<long> CreateProductAsync()
    {
        var payload = new
        {
            nameEn = $"Variant Product {Guid.NewGuid():N}",
            nameAr = (string?)null,
            categoryId = (long?)null,
            brandId = (long?)null,
            price = 100m
        };

        var response = await _client.PostAsJsonAsync("/api/products", payload);
        response.EnsureSuccessStatusCode();
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private async Task<JsonElement> CreateVariantAsync(long productId, string title)
    {
        var payload = new { title, stockQuantity = 5 };
        var response = await _client.PostAsJsonAsync($"/api/products/{productId}/variants", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
