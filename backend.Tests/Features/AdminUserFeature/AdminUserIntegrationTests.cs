using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.AdminUserFeature;

[Collection("Database")]
public class AdminUserIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AdminUserIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _client = _factory.CreateClient();
        await using var context = _fixture.CreateContext();
        await AuthTestHelper.SeedAdminUserAsync(context);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task POST_AuthLogin_ValidCredentials_Returns200WithAccessToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = AuthTestHelper.DefaultAdminEmail,
            password = AuthTestHelper.DefaultAdminPassword,
        });
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(
            body.GetProperty("data").GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task POST_AuthLogin_WrongPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = AuthTestHelper.DefaultAdminEmail,
            password = "WrongPassword123!",
        });
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task POST_AuthLogin_MissingFields_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_AuthLogout_WithValidToken_Returns200()
    {
        await using var context = _fixture.CreateContext();
        var authClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);

        var response = await authClient.PostAsync("/api/auth/logout", null);
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task GET_AuthMe_WithValidToken_Returns200WithUser()
    {
        await using var context = _fixture.CreateContext();
        var authClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);

        var response = await authClient.GetAsync("/api/auth/me");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(AuthTestHelper.DefaultAdminEmail,
            body.GetProperty("data").GetProperty("user").GetProperty("email").GetString());
    }

    [Fact]
    public async Task GET_Categories_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_Categories_WithValidToken_Returns200()
    {
        await using var context = _fixture.CreateContext();
        var authClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);

        var response = await authClient.GetAsync("/api/categories");
        var body = await ParseBody(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
    }

    private static async Task<JsonElement> ParseBody(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
    }
}
