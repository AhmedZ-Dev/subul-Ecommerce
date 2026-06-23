using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class BrandIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public BrandIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_Brands_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            name = $"Integration Brand {Guid.NewGuid():N}",
            descriptionEn = "Tech brand",
            isActive = true
        };

        var response = await _client.PostAsJsonAsync("/api/brands", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_Brands_MissingRequiredField_Returns400()
    {
        var payload = new { descriptionEn = "بدون اسم" };

        var response = await _client.PostAsJsonAsync("/api/brands", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GET_Brands_ById_ExistingId_Returns200WithBrand()
    {
        var created = await CreateBrandAsync($"GetById Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/brands/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_Brands_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/brands/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Brands_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/brands?page=1&limit=5");
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
    public async Task PUT_Brands_ValidUpdate_Returns200()
    {
        var created = await CreateBrandAsync($"Update Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedName = $"Updated Integration {Guid.NewGuid():N}";

        var payload = BuildUpdatePayload(updatedName);

        var response = await _client.PutAsJsonAsync($"/api/brands/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedName, body.GetProperty("data").GetProperty("name").GetString());
    }

    [Fact]
    public async Task PUT_Brands_NonExistentId_Returns404()
    {
        var payload = BuildUpdatePayload("Ghost Brand");

        var response = await _client.PutAsJsonAsync("/api/brands/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Brands_ExistingBrand_Returns200()
    {
        var created = await CreateBrandAsync($"Delete Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/brands/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Brands_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/brands/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateBrandAsync(string name)
    {
        var payload = new { name, isActive = true };
        var response = await _client.PostAsJsonAsync("/api/brands", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static object BuildUpdatePayload(string name) => new
    {
        name,
        slug = (string?)null,
        logoUrl = (string?)null,
        bannerUrl = (string?)null,
        descriptionEn = (string?)null,
        descriptionAr = (string?)null,
        websiteUrl = (string?)null,
        isFeatured = false,
        isActive = true,
        sortOrder = 0
    };

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
