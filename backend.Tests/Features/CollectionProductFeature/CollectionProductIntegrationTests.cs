using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CollectionProductFeature;

[Collection("Database")]
public class CollectionProductIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public CollectionProductIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_CollectionProducts_ValidPayload_Returns201()
    {
        var collectionId = await CreateCollectionAsync();
        var productId = await CreateProductAsync();

        var payload = new { productId, sortOrder = 0 };
        var response = await _client.PostAsJsonAsync($"/api/collections/{collectionId}/products", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(productId, body.GetProperty("data").GetProperty("productId").GetInt64());
    }

    [Fact]
    public async Task POST_CollectionProducts_NonExistentCollection_Returns404()
    {
        var productId = await CreateProductAsync();
        var payload = new { productId, sortOrder = 0 };

        var response = await _client.PostAsJsonAsync("/api/collections/999999999/products", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_CollectionProducts_List_Returns200WithPaginatedShape()
    {
        var collectionId = await CreateCollectionAsync();
        var productId = await CreateProductAsync();
        await _client.PostAsJsonAsync(
            $"/api/collections/{collectionId}/products",
            new { productId, sortOrder = 0 });

        var response = await _client.GetAsync($"/api/collections/{collectionId}/products?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
    }

    [Fact]
    public async Task GET_CollectionProducts_ById_ExistingId_Returns200()
    {
        var collectionId = await CreateCollectionAsync();
        var productId = await CreateProductAsync();
        var created = await _client.PostAsJsonAsync(
            $"/api/collections/{collectionId}/products",
            new { productId, sortOrder = 0 });
        var createdBody = await ParseBody(created);
        var linkId = createdBody.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/collections/{collectionId}/products/{linkId}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(linkId, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task PUT_CollectionProducts_ValidUpdate_Returns200()
    {
        var collectionId = await CreateCollectionAsync();
        var productId = await CreateProductAsync();
        var created = await _client.PostAsJsonAsync(
            $"/api/collections/{collectionId}/products",
            new { productId, sortOrder = 0 });
        var createdBody = await ParseBody(created);
        var linkId = createdBody.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.PutAsJsonAsync(
            $"/api/collections/{collectionId}/products/{linkId}",
            new { sortOrder = 3 });
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(3, body.GetProperty("data").GetProperty("sortOrder").GetInt32());
    }

    [Fact]
    public async Task DELETE_CollectionProducts_ExistingLink_Returns200()
    {
        var collectionId = await CreateCollectionAsync();
        var productId = await CreateProductAsync();
        var created = await _client.PostAsJsonAsync(
            $"/api/collections/{collectionId}/products",
            new { productId, sortOrder = 0 });
        var createdBody = await ParseBody(created);
        var linkId = createdBody.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/collections/{collectionId}/products/{linkId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_CollectionProducts_NonExistentId_Returns404()
    {
        var collectionId = await CreateCollectionAsync();

        var response = await _client.DeleteAsync($"/api/collections/{collectionId}/products/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<long> CreateCollectionAsync()
    {
        var payload = new
        {
            nameEn = $"Int Collection {Guid.NewGuid():N}",
            collectionType = "manual",
            isActive = true
        };

        var response = await _client.PostAsJsonAsync("/api/collections", payload);
        response.EnsureSuccessStatusCode();
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private async Task<long> CreateProductAsync()
    {
        var payload = new
        {
            nameEn = $"Int Product {Guid.NewGuid():N}",
            price = 100m
        };

        var response = await _client.PostAsJsonAsync("/api/products", payload);
        response.EnsureSuccessStatusCode();
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
