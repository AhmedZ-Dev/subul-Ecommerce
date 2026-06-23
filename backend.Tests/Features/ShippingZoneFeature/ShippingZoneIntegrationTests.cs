using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.ShippingZoneFeature;

[Collection("Database")]
public class ShippingZoneIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ShippingZoneIntegrationTests(DatabaseFixture fixture)
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
    public async Task POST_ShippingZones_ValidPayload_Returns201WithData()
    {
        var payload = new
        {
            nameEn = $"Integration Zone {Guid.NewGuid():N}",
            nameAr = "منطقة تكامل",
            governorates = new[] { "Baghdad", "Babylon" },
            isActive = true
        };

        var response = await _client.PostAsJsonAsync("/api/shipping-zones", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_ShippingZones_DuplicateName_Returns409()
    {
        var name = $"Conflict Zone {Guid.NewGuid():N}";
        var payload = new { nameEn = name };

        await _client.PostAsJsonAsync("/api/shipping-zones", payload);
        var response = await _client.PostAsJsonAsync("/api/shipping-zones", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_ShippingZones_ById_ExistingId_Returns200WithZone()
    {
        var created = await CreateZoneAsync($"GetById Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/shipping-zones/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_ShippingZones_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/shipping-zones/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_ShippingZones_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/shipping-zones?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
    }

    [Fact]
    public async Task PUT_ShippingZones_ValidUpdate_Returns200()
    {
        var created = await CreateZoneAsync($"Update Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedName = $"Updated Integration {Guid.NewGuid():N}";

        var payload = new
        {
            nameEn = updatedName,
            nameAr = (string?)null,
            governorates = new[] { "Anbar" },
            isActive = false,
            shippingRates = (object?)null
        };

        var response = await _client.PutAsJsonAsync($"/api/shipping-zones/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedName, body.GetProperty("data").GetProperty("nameEn").GetString());
    }

    [Fact]
    public async Task PUT_ShippingZones_NonExistentId_Returns404()
    {
        var payload = new
        {
            nameEn = "Ghost Zone",
            nameAr = (string?)null,
            governorates = (object?)null,
            isActive = true,
            shippingRates = (object?)null
        };

        var response = await _client.PutAsJsonAsync("/api/shipping-zones/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_ShippingZones_Existing_Returns200()
    {
        var created = await CreateZoneAsync($"Delete Integration {Guid.NewGuid():N}");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/shipping-zones/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_ShippingZones_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/shipping-zones/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateZoneAsync(string nameEn)
    {
        var payload = new { nameEn };
        var response = await _client.PostAsJsonAsync("/api/shipping-zones", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
