using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Features.CategoryFeature.CreateCategory;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CategoryFeature;

[Collection("Database")]
public class CategoryIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CategoryIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_Categories_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            nameEn = $"Integration Category {Guid.NewGuid():N}",
            nameAr = "تكامل",
            descriptionEn = (string?)null,
            descriptionAr = (string?)null,
            parentId = (long?)null
        };

        var response = await _client.PostAsJsonAsync("/api/categories", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_Categories_DuplicateName_Returns409()
    {
        var name = $"Conflict Cat {Guid.NewGuid():N}";
        var payload = new { nameEn = name, nameAr = (string?)null, parentId = (long?)null };

        await _client.PostAsJsonAsync("/api/categories", payload);
        var response = await _client.PostAsJsonAsync("/api/categories", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task POST_Categories_MissingRequiredField_Returns400()
    {
        var payload = new { nameAr = "بدون انجليزي" };

        var response = await _client.PostAsJsonAsync("/api/categories", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GET_Categories_ById_ExistingId_Returns200WithCategory()
    {
        var created = await CreateCategoryAsync($"GetById Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/categories/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_Categories_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/categories/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Categories_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/categories?page=1&limit=5");
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
    public async Task PUT_Categories_ValidUpdate_Returns200()
    {
        var created = await CreateCategoryAsync($"Update Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedName = $"Updated Integration {Guid.NewGuid():N}";

        var payload = new
        {
            nameEn = updatedName,
            nameAr = (string?)null,
            descriptionEn = (string?)null,
            descriptionAr = (string?)null,
            parentId = (long?)null,
            slug = (string?)null,
            imageUrl = (string?)null,
            bannerUrl = (string?)null,
            sortOrder = 0,
            isActive = true,
            seoTitle = (string?)null,
            seoDescription = (string?)null
        };

        var response = await _client.PutAsJsonAsync($"/api/categories/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedName, body.GetProperty("data").GetProperty("nameEn").GetString());
    }

    [Fact]
    public async Task PUT_Categories_NonExistentId_Returns404()
    {
        var payload = new
        {
            nameEn = "Ghost",
            nameAr = (string?)null, descriptionEn = (string?)null, descriptionAr = (string?)null,
            parentId = (long?)null, slug = (string?)null, imageUrl = (string?)null,
            bannerUrl = (string?)null, sortOrder = 0, isActive = true,
            seoTitle = (string?)null, seoDescription = (string?)null
        };

        var response = await _client.PutAsJsonAsync("/api/categories/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Categories_ExistingLeafCategory_Returns200()
    {
        var created = await CreateCategoryAsync($"Delete Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/categories/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Categories_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/categories/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Categories_Status_Deactivate_Returns200()
    {
        var created = await CreateCategoryAsync($"Status Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.PutAsJsonAsync($"/api/categories/{id}/status", new { isActive = false });
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.False(body.GetProperty("data").GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task PUT_Categories_Status_NonExistentId_Returns404()
    {
        var response = await _client.PutAsJsonAsync("/api/categories/999999999/status", new { isActive = false });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateCategoryAsync(string nameEn)
    {
        var payload = new { nameEn, nameAr = (string?)null, parentId = (long?)null };
        var response = await _client.PostAsJsonAsync("/api/categories", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
