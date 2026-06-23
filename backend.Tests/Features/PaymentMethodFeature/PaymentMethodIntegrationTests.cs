using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

[Collection("Database")]
public class PaymentMethodIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public PaymentMethodIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_PaymentMethods_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            name = $"cod_{Guid.NewGuid():N}",
            labelEn = "Cash on Delivery",
            labelAr = "الدفع عند الاستلام",
            type = "offline",
            isActive = true,
            sortOrder = 0
        };

        var response = await _client.PostAsJsonAsync("/api/payment-methods", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("offline", body.GetProperty("data").GetProperty("type").GetString());
    }

    [Fact]
    public async Task GET_PaymentMethods_List_Returns200WithPaginatedShape()
    {
        await CreatePaymentMethodAsync();

        var response = await _client.GetAsync("/api/payment-methods?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
    }

    [Fact]
    public async Task GET_PaymentMethods_ById_ExistingId_Returns200()
    {
        var created = await CreatePaymentMethodAsync();
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/payment-methods/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_PaymentMethods_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/payment-methods/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task PUT_PaymentMethods_ValidUpdate_Returns200()
    {
        var created = await CreatePaymentMethodAsync();
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var name = created.GetProperty("data").GetProperty("name").GetString();

        var payload = new
        {
            name,
            labelEn = "Updated Label",
            labelAr = (string?)null,
            type = "offline",
            gateway = (string?)null,
            gatewayConfig = (string?)null,
            iconUrl = (string?)null,
            instructionsEn = "Updated instructions",
            instructionsAr = (string?)null,
            isActive = true,
            sortOrder = 2
        };

        var response = await _client.PutAsJsonAsync($"/api/payment-methods/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("Updated Label", body.GetProperty("data").GetProperty("labelEn").GetString());
    }

    [Fact]
    public async Task DELETE_PaymentMethods_ExistingMethod_Returns200()
    {
        var created = await CreatePaymentMethodAsync();
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/payment-methods/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_PaymentMethods_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/payment-methods/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreatePaymentMethodAsync()
    {
        var payload = new
        {
            name = $"int_{Guid.NewGuid():N}",
            labelEn = "Integration Method",
            type = "offline",
            isActive = true
        };

        var response = await _client.PostAsJsonAsync("/api/payment-methods", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
