using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.ProductImageFeature;

[Collection("Database")]
public class ProductImageIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public ProductImageIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_Images_ValidFile_Returns201WithData()
    {
        var productId = await CreateProductAsync();
        using var content = CreateMultipartContent("Front view", 0, true);

        var response = await _client.PostAsync($"/api/products/{productId}/images", content);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(productId, body.GetProperty("data").GetProperty("productId").GetInt64());
        Assert.StartsWith($"/img/products/{productId}/", body.GetProperty("data").GetProperty("imageUrl").GetString());
    }

    [Fact]
    public async Task POST_Images_NonExistentProduct_Returns404()
    {
        using var content = CreateMultipartContent(null, 0, false);

        var response = await _client.PostAsync("/api/products/999999999/images", content);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Images_List_Returns200WithPaginatedShape()
    {
        var productId = await CreateProductAsync();
        using var content = CreateMultipartContent(null, 0, true);
        await _client.PostAsync($"/api/products/{productId}/images", content);

        var response = await _client.GetAsync($"/api/products/{productId}/images?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
    }

    [Fact]
    public async Task GET_Images_ById_ExistingId_Returns200()
    {
        var productId = await CreateProductAsync();
        using var content = CreateMultipartContent("ById", 0, false);
        var created = await _client.PostAsync($"/api/products/{productId}/images", content);
        var createdBody = await ParseBody(created);
        var imageId = createdBody.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/products/{productId}/images/{imageId}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(imageId, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task PUT_Images_ValidUpdate_Returns200()
    {
        var productId = await CreateProductAsync();
        using var content = CreateMultipartContent("Before", 0, false);
        var created = await _client.PostAsync($"/api/products/{productId}/images", content);
        var createdBody = await ParseBody(created);
        var imageId = createdBody.GetProperty("data").GetProperty("id").GetInt64();

        var payload = new
        {
            variantId = (long?)null,
            altText = "After update",
            sortOrder = 3,
            isPrimary = true
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{productId}/images/{imageId}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("After update", body.GetProperty("data").GetProperty("altText").GetString());
    }

    [Fact]
    public async Task DELETE_Images_ExistingImage_Returns200()
    {
        var productId = await CreateProductAsync();
        using var content = CreateMultipartContent(null, 0, false);
        var created = await _client.PostAsync($"/api/products/{productId}/images", content);
        var createdBody = await ParseBody(created);
        var imageId = createdBody.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/products/{productId}/images/{imageId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Images_NonExistentId_Returns404()
    {
        var productId = await CreateProductAsync();

        var response = await _client.DeleteAsync($"/api/products/{productId}/images/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<long> CreateProductAsync()
    {
        var payload = new
        {
            nameEn = $"Image Product {Guid.NewGuid():N}",
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

    private static MultipartFormDataContent CreateMultipartContent(string? altText, int sortOrder, bool isPrimary)
    {
        var content = new MultipartFormDataContent();
        var pngBytes = ProductImageTestHelpers.CreatePngFormFile();
        var streamContent = new StreamContent(pngBytes.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(streamContent, "Image", "test.png");

        if (altText is not null)
            content.Add(new StringContent(altText), "AltText");

        content.Add(new StringContent(sortOrder.ToString()), "SortOrder");
        content.Add(new StringContent(isPrimary.ToString()), "IsPrimary");

        return content;
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
