using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.CartFeature;

[Collection("Database")]
public class CartIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CartIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_CartItems_ValidPayload_Returns201WithSessionHeader()
    {
        var productId = await CreateProductAsync();
        var sessionId = Guid.NewGuid().ToString("N");
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/carts/items");
        request.Headers.Add("X-Cart-Session", sessionId);
        request.Content = JsonContent.Create(new { productId, quantity = 1 });

        var response = await _client.SendAsync(request);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(response.Headers.Contains("X-Cart-Session"));
    }

    [Fact]
    public async Task GET_Cart_WithSession_Returns200()
    {
        var sessionId = Guid.NewGuid().ToString("N");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/carts");
        request.Headers.Add("X-Cart-Session", sessionId);

        var response = await _client.SendAsync(request);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
    }

    private async Task<long> CreateProductAsync()
    {
        var payload = new
        {
            nameEn = $"Cart Integration {Guid.NewGuid():N}",
            price = 150,
            stockQuantity = 20
        };

        var response = await _client.PostAsJsonAsync("/api/products", payload);
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
    }
}
