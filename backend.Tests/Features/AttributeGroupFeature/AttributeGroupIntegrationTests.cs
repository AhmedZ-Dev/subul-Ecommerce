using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AttributeGroupFeature;

[Collection("Database")]
public class AttributeGroupIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AttributeGroupIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_AttributeGroups_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            nameEn = $"Integration Group {Guid.NewGuid():N}",
            nameAr = "مجموعة تكامل",
            sortOrder = 1,
            isFilterable = true
        };

        var response = await _client.PostAsJsonAsync("/api/attribute-groups", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_AttributeGroups_DuplicateName_Returns409()
    {
        var name = $"Conflict Group {Guid.NewGuid():N}";
        var payload = new { nameEn = name };

        await _client.PostAsJsonAsync("/api/attribute-groups", payload);
        var response = await _client.PostAsJsonAsync("/api/attribute-groups", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_AttributeGroups_ById_ExistingId_Returns200WithGroup()
    {
        var created = await CreateGroupAsync($"GetById Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/attribute-groups/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_AttributeGroups_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/attribute-groups/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_AttributeGroups_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/attribute-groups?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
    }

    [Fact]
    public async Task PUT_AttributeGroups_ValidUpdate_Returns200()
    {
        var created = await CreateGroupAsync($"Update Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedName = $"Updated Integration {Guid.NewGuid():N}";

        var payload = new
        {
            nameEn = updatedName,
            nameAr = (string?)null,
            slug = (string?)null,
            sortOrder = 5,
            isFilterable = false,
            attributes = (object?)null
        };

        var response = await _client.PutAsJsonAsync($"/api/attribute-groups/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedName, body.GetProperty("data").GetProperty("nameEn").GetString());
    }

    [Fact]
    public async Task PUT_AttributeGroups_NonExistentId_Returns404()
    {
        var payload = new
        {
            nameEn = "Ghost Group",
            nameAr = (string?)null,
            slug = (string?)null,
            sortOrder = 0,
            isFilterable = true,
            attributes = (object?)null
        };

        var response = await _client.PutAsJsonAsync("/api/attribute-groups/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_AttributeGroups_Existing_Returns200()
    {
        var created = await CreateGroupAsync($"Delete Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/attribute-groups/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_AttributeGroups_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/attribute-groups/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateGroupAsync(string nameEn)
    {
        var payload = new { nameEn };
        var response = await _client.PostAsJsonAsync("/api/attribute-groups", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
