using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.CollectionFeature;

[Collection("Database")]
public class CollectionIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CollectionIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_Collections_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            nameEn = $"Integration Collection {Guid.NewGuid():N}",
            nameAr = "تجميعة تكامل",
            collectionType = "manual",
            isActive = true
        };

        var response = await _client.PostAsJsonAsync("/api/collections", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_Collections_DuplicateName_Returns409()
    {
        var name = $"Conflict Col {Guid.NewGuid():N}";
        var payload = new { nameEn = name, collectionType = "manual" };

        await _client.PostAsJsonAsync("/api/collections", payload);
        var response = await _client.PostAsJsonAsync("/api/collections", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Collections_ById_ExistingId_Returns200WithCollection()
    {
        var created = await CreateCollectionAsync($"GetById Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/collections/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_Collections_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/collections/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Collections_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/collections?page=1&limit=5");
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
    public async Task PUT_Collections_ValidUpdate_Returns200()
    {
        var created = await CreateCollectionAsync($"Update Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedName = $"Updated Integration {Guid.NewGuid():N}";

        var payload = new
        {
            nameEn = updatedName,
            nameAr = (string?)null,
            descriptionEn = (string?)null,
            descriptionAr = (string?)null,
            imageUrl = (string?)null,
            bannerUrl = (string?)null,
            collectionType = "smart",
            isActive = false,
            sortOrder = 10,
            metaTitle = (string?)null,
            metaDescription = (string?)null,
            products = (object?)null,
            slug = (string?)null
        };

        var response = await _client.PutAsJsonAsync($"/api/collections/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedName, body.GetProperty("data").GetProperty("nameEn").GetString());
    }

    [Fact]
    public async Task PUT_Collections_NonExistentId_Returns404()
    {
        var payload = new
        {
            nameEn = "Ghost",
            nameAr = (string?)null,
            descriptionEn = (string?)null,
            descriptionAr = (string?)null,
            imageUrl = (string?)null,
            bannerUrl = (string?)null,
            collectionType = "manual",
            isActive = true,
            sortOrder = 0,
            metaTitle = (string?)null,
            metaDescription = (string?)null,
            products = (object?)null,
            slug = (string?)null
        };

        var response = await _client.PutAsJsonAsync("/api/collections/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Collections_ExistingCategory_Returns200()
    {
        var created = await CreateCollectionAsync($"Delete Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/collections/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Collections_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/collections/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateCollectionAsync(string nameEn)
    {
        var payload = new { nameEn, collectionType = "manual" };
        var response = await _client.PostAsJsonAsync("/api/collections", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
