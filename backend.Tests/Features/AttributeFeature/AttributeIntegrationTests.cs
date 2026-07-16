using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AttributeFeature;

[Collection("Database")]
public class AttributeIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public AttributeIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_Attributes_ValidPayload_Returns201WithData()
    {
        var groupId = await CreateGroupAsync();
        var payload = new
        {
            nameEn = "RAM",
            nameAr = "الذاكرة",
            unit = "GB",
            inputType = "select",
            isFilterable = true,
            sortOrder = 1
        };

        var response = await _client.PostAsJsonAsync($"/api/attribute-groups/{groupId}/attributes", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(groupId, body.GetProperty("data").GetProperty("groupId").GetInt64());
    }

    [Fact]
    public async Task POST_Attributes_NonExistentGroup_Returns404()
    {
        var payload = new { nameEn = "Orphan Attribute", inputType = "text" };

        var response = await _client.PostAsJsonAsync("/api/attribute-groups/999999999/attributes", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Attributes_List_Returns200WithPaginatedShape()
    {
        var groupId = await CreateGroupAsync();
        await _client.PostAsJsonAsync(
            $"/api/attribute-groups/{groupId}/attributes",
            new { nameEn = "GPU", inputType = "text" });

        var response = await _client.GetAsync($"/api/attribute-groups/{groupId}/attributes?page=1&limit=5");
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
    public async Task GET_Attributes_ById_ExistingId_Returns200()
    {
        var groupId = await CreateGroupAsync();
        var created = await CreateAttributeAsync(groupId, "Processor");
        var attributeId = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/attribute-groups/{groupId}/attributes/{attributeId}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(attributeId, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_Attributes_ById_NonExistentId_Returns404()
    {
        var groupId = await CreateGroupAsync();

        var response = await _client.GetAsync($"/api/attribute-groups/{groupId}/attributes/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task PUT_Attributes_ValidUpdate_Returns200()
    {
        var groupId = await CreateGroupAsync();
        var created = await CreateAttributeAsync(groupId, "Before Update");
        var attributeId = created.GetProperty("data").GetProperty("id").GetInt64();

        var payload = new
        {
            nameEn = "After Update",
            nameAr = (string?)null,
            slug = (string?)null,
            unit = "Hz",
            inputType = "number",
            isFilterable = true,
            sortOrder = 2
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/attribute-groups/{groupId}/attributes/{attributeId}",
            payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("After Update", body.GetProperty("data").GetProperty("nameEn").GetString());
    }

    [Fact]
    public async Task DELETE_Attributes_ExistingAttribute_Returns200()
    {
        var groupId = await CreateGroupAsync();
        var created = await CreateAttributeAsync(groupId, "Delete Attribute");
        var attributeId = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/attribute-groups/{groupId}/attributes/{attributeId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Attributes_NonExistentId_Returns404()
    {
        var groupId = await CreateGroupAsync();

        var response = await _client.DeleteAsync($"/api/attribute-groups/{groupId}/attributes/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<long> CreateGroupAsync()
    {
        var payload = new
        {
            nameEn = $"Attribute Group {Guid.NewGuid():N}",
            nameAr = (string?)null,
            sortOrder = 0,
            isFilterable = true
        };

        var response = await _client.PostAsJsonAsync("/api/attribute-groups", payload);
        response.EnsureSuccessStatusCode();
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private async Task<JsonElement> CreateAttributeAsync(long groupId, string nameEn)
    {
        var payload = new { nameEn, inputType = "text", isFilterable = true, sortOrder = 0 };
        var response = await _client.PostAsJsonAsync($"/api/attribute-groups/{groupId}/attributes", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
