using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.BrandFeature;

[Collection("Database")]
public class BrandImageIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public BrandImageIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_Logo_ValidFile_Returns201WithData()
    {
        var brandId = await CreateBrandAsync();
        using var content = CreateMultipartContent();

        var response = await _client.PostAsync($"/api/brands/{brandId}/logo", content);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.StartsWith(
            $"/img/brands/{brandId}/logo-",
            body.GetProperty("data").GetProperty("logoUrl").GetString());
    }

    [Fact]
    public async Task POST_Logo_NonExistentBrand_Returns404()
    {
        using var content = CreateMultipartContent();

        var response = await _client.PostAsync("/api/brands/999999999/logo", content);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task POST_Logo_InvalidFile_Returns400()
    {
        var brandId = await CreateBrandAsync();
        using var content = CreateInvalidMultipartContent();

        var response = await _client.PostAsync($"/api/brands/{brandId}/logo", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Logo_ReplacesPreviousLogo()
    {
        var brandId = await CreateBrandAsync();
        using var first = CreateMultipartContent();
        using var second = CreateMultipartContent();

        var firstResponse = await _client.PostAsync($"/api/brands/{brandId}/logo", first);
        var firstBody = await ParseBody(firstResponse);
        var firstLogoUrl = firstBody.GetProperty("data").GetProperty("logoUrl").GetString();

        var secondResponse = await _client.PostAsync($"/api/brands/{brandId}/logo", second);
        var secondBody = await ParseBody(secondResponse);
        var secondLogoUrl = secondBody.GetProperty("data").GetProperty("logoUrl").GetString();

        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        Assert.NotEqual(firstLogoUrl, secondLogoUrl);
    }

    [Fact]
    public async Task POST_Logo_ReplacesPreviousLogo_DeletesOldFileFromDisk()
    {
        var brandId = await CreateBrandAsync();
        using var first = CreateMultipartContent();
        using var second = CreateMultipartContent();

        var firstResponse = await _client.PostAsync($"/api/brands/{brandId}/logo", first);
        var firstBody = await ParseBody(firstResponse);
        var firstLogoUrl = firstBody.GetProperty("data").GetProperty("logoUrl").GetString()!;
        var firstPhysicalPath = _factory.GetPhysicalPathForRelativeUrl(firstLogoUrl);

        Assert.True(File.Exists(firstPhysicalPath));

        var secondResponse = await _client.PostAsync($"/api/brands/{brandId}/logo", second);
        var secondBody = await ParseBody(secondResponse);
        var secondLogoUrl = secondBody.GetProperty("data").GetProperty("logoUrl").GetString()!;

        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        Assert.False(File.Exists(firstPhysicalPath));
        Assert.True(File.Exists(_factory.GetPhysicalPathForRelativeUrl(secondLogoUrl)));
    }

    [Fact]
    public async Task POST_Logo_FakePngExtension_Returns400()
    {
        var brandId = await CreateBrandAsync();
        using var content = CreateFakePngMultipartContent();

        var response = await _client.PostAsync($"/api/brands/{brandId}/logo", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Logo_ExistingBrand_Returns200AndClearsLogo()
    {
        var brandId = await CreateBrandAsync();
        using var content = CreateMultipartContent();
        await _client.PostAsync($"/api/brands/{brandId}/logo", content);

        var response = await _client.DeleteAsync($"/api/brands/{brandId}/logo");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("logoUrl").ValueKind == JsonValueKind.Null);
    }

    [Fact]
    public async Task POST_Banner_ValidFile_Returns201WithData()
    {
        var brandId = await CreateBrandAsync();
        using var content = CreateMultipartContent();

        var response = await _client.PostAsync($"/api/brands/{brandId}/banner", content);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.StartsWith(
            $"/img/brands/{brandId}/banner-",
            body.GetProperty("data").GetProperty("bannerUrl").GetString());
    }

    private async Task<long> CreateBrandAsync()
    {
        var payload = new { name = $"Brand Image {Guid.NewGuid():N}", isActive = true };
        var response = await _client.PostAsJsonAsync("/api/brands", payload);
        response.EnsureSuccessStatusCode();
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private static MultipartFormDataContent CreateMultipartContent()
    {
        var content = new MultipartFormDataContent();
        var pngFile = ProductImageTestHelpers.CreatePngFormFile();
        var streamContent = new StreamContent(pngFile.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(streamContent, "Image", "test.png");
        return content;
    }

    private static MultipartFormDataContent CreateInvalidMultipartContent()
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent("not an image"u8.ToArray());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "Image", "test.txt");
        return content;
    }

    private static MultipartFormDataContent CreateFakePngMultipartContent()
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent("not an image"u8.ToArray());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "Image", "fake.png");
        return content;
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
