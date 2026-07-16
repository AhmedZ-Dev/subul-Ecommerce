using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.OrderFeature;

[Collection("Database")]
public class OrderIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private HttpClient _adminClient = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrderIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _client = _factory.CreateClient();
        await using var context = _fixture.CreateContext();
        _adminClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        _adminClient.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task POST_Orders_GuestCheckout_Returns201()
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var zoneId = await CreateShippingZoneAsync();
        var productId = await CreateProductAsync();

        using var addRequest = new HttpRequestMessage(HttpMethod.Post, "/api/carts/items");
        addRequest.Headers.Add("X-Cart-Session", sessionId);
        addRequest.Content = JsonContent.Create(new { productId, quantity = 1 });
        await _client.SendAsync(addRequest);

        using var orderRequest = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        orderRequest.Headers.Add("X-Cart-Session", sessionId);
        orderRequest.Content = JsonContent.Create(new
        {
            shippingFirstName = "Guest",
            shippingPhone = "07701112222",
            shippingAddress1 = "Main Street",
            shippingCity = "Baghdad",
            shippingGovernorate = "Baghdad",
            shippingZoneId = zoneId,
            paymentMethod = "cod"
        });

        var response = await _client.SendAsync(orderRequest);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.StartsWith("ORD-", body.GetProperty("data").GetProperty("orderNumber").GetString());
    }

    [Fact]
    public async Task GET_OrdersTrack_ValidCredentials_Returns200()
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var zoneId = await CreateShippingZoneAsync();
        var productId = await CreateProductAsync();
        const string phone = "07703334444";

        using var addRequest = new HttpRequestMessage(HttpMethod.Post, "/api/carts/items");
        addRequest.Headers.Add("X-Cart-Session", sessionId);
        addRequest.Content = JsonContent.Create(new { productId, quantity = 1 });
        await _client.SendAsync(addRequest);

        using var orderRequest = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        orderRequest.Headers.Add("X-Cart-Session", sessionId);
        orderRequest.Content = JsonContent.Create(new
        {
            shippingFirstName = "Tracker",
            shippingPhone = phone,
            shippingAddress1 = "Track Street",
            shippingCity = "Baghdad",
            shippingGovernorate = "Baghdad",
            shippingZoneId = zoneId
        });

        var orderResponse = await _client.SendAsync(orderRequest);
        var orderBody = await ParseBody(orderResponse);
        var orderNumber = orderBody.GetProperty("data").GetProperty("orderNumber").GetString();

        var trackResponse = await _client.GetAsync($"/api/orders/track?orderNumber={orderNumber}&phone={phone}");
        var trackBody = await ParseBody(trackResponse);

        Assert.Equal(HttpStatusCode.OK, trackResponse.StatusCode);
        Assert.True(trackBody.GetProperty("success").GetBoolean());
        Assert.Equal(orderNumber, trackBody.GetProperty("data").GetProperty("orderNumber").GetString());
    }

    [Fact]
    public async Task GET_OrderById_Returns200WithItems()
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var zoneId = await CreateShippingZoneAsync();
        var productId = await CreateProductAsync();

        using var addRequest = new HttpRequestMessage(HttpMethod.Post, "/api/carts/items");
        addRequest.Headers.Add("X-Cart-Session", sessionId);
        addRequest.Content = JsonContent.Create(new { productId, quantity = 1 });
        await _client.SendAsync(addRequest);

        using var orderRequest = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        orderRequest.Headers.Add("X-Cart-Session", sessionId);
        orderRequest.Content = JsonContent.Create(new
        {
            shippingFirstName = "Reader",
            shippingPhone = "07705556666",
            shippingAddress1 = "Read Street",
            shippingCity = "Baghdad",
            shippingGovernorate = "Baghdad",
            shippingZoneId = zoneId
        });

        var orderResponse = await _client.SendAsync(orderRequest);
        var orderBody = await ParseBody(orderResponse);
        var orderId = orderBody.GetProperty("data").GetProperty("id").GetInt64();

        var getResponse = await _adminClient.GetAsync($"/api/orders/{orderId}");
        var getBody = await ParseBody(getResponse);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.True(getBody.GetProperty("success").GetBoolean());
        Assert.Equal(1, getBody.GetProperty("data").GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task GET_OrderItems_Returns200()
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var zoneId = await CreateShippingZoneAsync();
        var productId = await CreateProductAsync();

        using var addRequest = new HttpRequestMessage(HttpMethod.Post, "/api/carts/items");
        addRequest.Headers.Add("X-Cart-Session", sessionId);
        addRequest.Content = JsonContent.Create(new { productId, quantity = 2 });
        await _client.SendAsync(addRequest);

        using var orderRequest = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        orderRequest.Headers.Add("X-Cart-Session", sessionId);
        orderRequest.Content = JsonContent.Create(new
        {
            shippingFirstName = "Items",
            shippingPhone = "07707778888",
            shippingAddress1 = "Items Street",
            shippingCity = "Baghdad",
            shippingGovernorate = "Baghdad",
            shippingZoneId = zoneId
        });

        var orderResponse = await _client.SendAsync(orderRequest);
        var orderBody = await ParseBody(orderResponse);
        var orderId = orderBody.GetProperty("data").GetProperty("id").GetInt64();

        var itemsResponse = await _adminClient.GetAsync($"/api/orders/{orderId}/items");
        var itemsBody = await ParseBody(itemsResponse);

        Assert.Equal(HttpStatusCode.OK, itemsResponse.StatusCode);
        Assert.True(itemsBody.GetProperty("success").GetBoolean());
        Assert.Equal(1, itemsBody.GetProperty("data").GetProperty("items").GetArrayLength());
        Assert.Equal(2, itemsBody.GetProperty("data").GetProperty("items")[0].GetProperty("quantity").GetInt32());
    }

    private async Task<long> CreateProductAsync()
    {
        var payload = new
        {
            nameEn = $"Order Integration {Guid.NewGuid():N}",
            price = 200,
            stockQuantity = 10
        };

        var response = await _adminClient.PostAsJsonAsync("/api/products", payload);
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private async Task<long> CreateShippingZoneAsync()
    {
        var payload = new
        {
            nameEn = $"Zone {Guid.NewGuid():N}",
            governorates = new[] { "Baghdad" },
            shippingRates = new[]
            {
                new
                {
                    nameEn = "Flat",
                    rateType = "flat",
                    price = 4000m,
                    isActive = true
                }
            }
        };

        var response = await _adminClient.PostAsJsonAsync("/api/shipping-zones", payload);
        var body = await ParseBody(response);
        return body.GetProperty("data").GetProperty("id").GetInt64();
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
    }
}
