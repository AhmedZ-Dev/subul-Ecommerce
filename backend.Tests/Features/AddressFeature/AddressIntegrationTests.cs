using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AddressFeature;

[Collection("Database")]
public class AddressIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AddressIntegrationTests(DatabaseFixture fixture)
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

    private async Task<long> CreateTestUserAsync()
    {
        // Add a user directly through DbContext to use its ID
        await using var context = _fixture.CreateContext();
        var user = new Domain.Entities.User
        {
            Email = $"integration_user_{Guid.NewGuid():N}@test.com",
            StoreCredit = 0m,
            CreatedAt = DateTime.Now
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    [Fact]
    public async Task POST_Addresses_ValidPayload_Returns201WithData()
    {
        var userId = await CreateTestUserAsync();
        var payload = new
        {
            userId,
            firstName = "Ahmed",
            lastName = "Z",
            phone = "+9647700000000",
            address1 = "Baghdad, Mansour",
            address2 = (string?)null,
            city = "Baghdad",
            governorate = "Baghdad",
            country = "Iraq",
            isDefault = true
        };

        var response = await _client.PostAsJsonAsync("/api/addresses", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.True(body.GetProperty("data").GetProperty("id").GetInt64() > 0);
    }

    [Fact]
    public async Task POST_Addresses_NonExistentUser_Returns404()
    {
        var payload = new
        {
            userId = 99999999,
            address1 = "Nowhere",
            country = "Iraq"
        };

        var response = await _client.PostAsJsonAsync("/api/addresses", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Addresses_ById_ExistingId_Returns200WithAddress()
    {
        var userId = await CreateTestUserAsync();
        var created = await CreateAddressAsync(userId, "GetById Road");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.GetAsync($"/api/addresses/{id}");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(id, body.GetProperty("data").GetProperty("id").GetInt64());
    }

    [Fact]
    public async Task GET_Addresses_ById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/addresses/999999999");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_Addresses_List_Returns200WithPaginatedShape()
    {
        var response = await _client.GetAsync("/api/addresses?page=1&limit=5");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());

        var data = body.GetProperty("data");
        Assert.True(data.TryGetProperty("items", out _));
        Assert.True(data.TryGetProperty("total", out _));
    }

    [Fact]
    public async Task PUT_Addresses_ValidUpdate_Returns200()
    {
        var userId = await CreateTestUserAsync();
        var created = await CreateAddressAsync(userId, "Update Street");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();
        var updatedRoad = "Updated Road 123";

        var payload = new
        {
            firstName = "Ahmed",
            lastName = "Z",
            phone = "123",
            address1 = updatedRoad,
            address2 = (string?)null,
            city = "Baghdad",
            governorate = "Baghdad",
            country = "Iraq",
            isDefault = true
        };

        var response = await _client.PutAsJsonAsync($"/api/addresses/{id}", payload);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(updatedRoad, body.GetProperty("data").GetProperty("address1").GetString());
    }

    [Fact]
    public async Task PUT_Addresses_NonExistentId_Returns404()
    {
        var payload = new
        {
            firstName = "Ghost",
            address1 = "Ghost",
            country = "Iraq",
            isDefault = true
        };

        var response = await _client.PutAsJsonAsync("/api/addresses/999999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Addresses_Existing_Returns200()
    {
        var userId = await CreateTestUserAsync();
        var created = await CreateAddressAsync(userId, "Delete Street");
        var id = created.GetProperty("data").GetProperty("id").GetInt64();

        var response = await _client.DeleteAsync($"/api/addresses/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Addresses_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/addresses/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonElement> CreateAddressAsync(long userId, string address1)
    {
        var payload = new { userId, address1, country = "Iraq" };
        var response = await _client.PostAsJsonAsync("/api/addresses", payload);
        response.EnsureSuccessStatusCode();
        return await ParseBody(response);
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
