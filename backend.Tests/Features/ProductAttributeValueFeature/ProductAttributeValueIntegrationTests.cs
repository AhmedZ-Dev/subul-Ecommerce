using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductAttributeValueFeature;

[Collection("Database")]
public class ProductAttributeValueIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public ProductAttributeValueIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_AttributeValues_ValidPayload_Returns201WithData()
    {
        var productId = await CreateProductAsync();
        var attributeId = await CreateTextAttributeAsync();
        var payload = new { attributeId, valueText = "16GB" };

        var response = await _client.PostAsJsonAsync($"/api/products/{productId}/attribute-values", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(productId, body.GetProperty("data").GetProperty("productId").GetInt64());
        Assert.Equal("16GB", body.GetProperty("data").GetProperty("valueText").GetString());
    }

    [Fact]
    public async Task POST_AttributeValues_NonExistentProduct_Returns404()
    {
        var attributeId = await CreateTextAttributeAsync();
        var payload = new { attributeId, valueText = "16GB" };

        var response = await _client.PostAsJsonAsync("/api/products/999999999/attribute-values", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_AttributeValues_List_Returns200WithPaginatedShape()
    {
        var productId = await CreateProductAsync();
        var attributeId = await CreateTextAttributeAsync();
        await _client.PostAsJsonAsync(
            $"/api/products/{productId}/attribute-values",
            new { attributeId, valueText = "512GB SSD" });

        var response = await _client.GetAsync($"/api/products/{productId}/attribute-values?page=1&limit=5");
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
    public async Task GET_AttributeValues_ById_ExistingId_Returns200()
    {
        var productId = await CreateProductAsync();
        var created = await CreateAttributeValueAsync(productId, "Core i5");
        var valueId = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/products/{productId}/attribute-values/{valueId}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(valueId, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_AttributeValues_ById_NonExistentId_Returns404()
    {
        var productId = await CreateProductAsync();

        var response = await _client.GetAsync($"/api/products/{productId}/attribute-values/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task PUT_AttributeValues_ValidUpdate_Returns200()
    {
        var productId = await CreateProductAsync();
        var created = await CreateAttributeValueAsync(productId, "512GB SSD");
        var valueId = created.GetProperty("data").GetProperty("id").GetInt64();
        var attributeId = created.GetProperty("data").GetProperty("attributeId").GetInt64();

        var payload = new
        {
            attributeId,
            valueText = "1TB SSD",
            valueNumber = (decimal?)null,
            valueBoolean = (bool?)null
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/products/{productId}/attribute-values/{valueId}",
            payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("1TB SSD", body.GetProperty("data").GetProperty("valueText").GetString());
    }

    [Fact]
    public async Task DELETE_AttributeValues_ExistingValue_Returns200()
    {
        var productId = await CreateProductAsync();
        var created = await CreateAttributeValueAsync(productId, "Delete Value");
        var valueId = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/products/{productId}/attribute-values/{valueId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_AttributeValues_NonExistentId_Returns404()
    {
        var productId = await CreateProductAsync();

        var response = await _client.DeleteAsync($"/api/products/{productId}/attribute-values/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<long> CreateProductAsync()
    {
        var payload = new
        {
            nameEn = $"PAV Product {Guid.NewGuid():N}",
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

    private async Task<long> CreateTextAttributeAsync()
    {
        var groupPayload = new
        {
            nameEn = $"PAV Group {Guid.NewGuid():N}",
            nameAr = (string?)null,
            sortOrder = 0,
            isFilterable = true
        };

        var groupResponse = await _client.PostAsJsonAsync("/api/attribute-groups", groupPayload);
        groupResponse.EnsureSuccessStatusCode();
        var groupBody = await ParseBody(groupResponse);
        var groupId = groupBody.GetProperty("data").GetProperty("id").GetInt64();

        var attributePayload = new
        {
            nameEn = $"RAM {Guid.NewGuid():N}",
            inputType = "text",
            isFilterable = true,
            sortOrder = 0
        };

        var attributeResponse = await _client.PostAsJsonAsync(
            $"/api/attribute-groups/{groupId}/attributes",
            attributePayload);
        attributeResponse.EnsureSuccessStatusCode();
        var attributeBody = await ParseBody(attributeResponse);
        return attributeBody.GetProperty("data").GetProperty("id").GetInt64();
    }

    private async Task<JsonElement> CreateAttributeValueAsync(long productId, string valueText)
    {
        var attributeId = await CreateTextAttributeAsync();
        var payload = new { attributeId, valueText };
        var response = await _client.PostAsJsonAsync($"/api/products/{productId}/attribute-values", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
