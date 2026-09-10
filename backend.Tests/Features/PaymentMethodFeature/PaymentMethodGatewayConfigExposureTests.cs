using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Domain.Entities;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.PaymentMethodFeature;

/// <summary>
/// Regression tests for the Wave B finding C-7.
///
/// GET /api/payment-methods and /api/payment-methods/{id} were [AllowAnonymous]
/// and mapped the entity straight through, so <c>gatewayConfig</c> — the field the
/// admin form labels "إعدادات البوابة (JSON)" with an <c>{"apiKey": "..."}</c>
/// placeholder — was readable by anyone on the internet. The storefront now reads
/// /api/payment-methods/public, which projects only publishable fields.
/// </summary>
[Collection("Database")]
public class PaymentMethodGatewayConfigExposureTests : IAsyncLifetime
{
    private const string SecretMarker = "sk_live_MUST_NEVER_BE_PUBLIC";

    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _anonymousClient = null!;
    private HttpClient _adminClient = null!;
    private long _methodId;

    public PaymentMethodGatewayConfigExposureTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _anonymousClient = _factory.CreateClient();

        await using var context = _fixture.CreateContext();
        _adminClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);

        var method = new PaymentMethod
        {
            Name = $"gateway_{Guid.NewGuid():N}",
            LabelEn = "Card",
            LabelAr = "بطاقة",
            Type = "online",
            Gateway = "some-processor",
            GatewayConfig = $$"""{"apiKey":"{{SecretMarker}}","webhookSecret":"whsec_abc"}""",
            IsActive = true,
            SortOrder = 0,
            CreatedAt = DateTime.Now,
        };
        context.PaymentMethods.Add(method);
        await context.SaveChangesAsync();
        _methodId = method.Id;
    }

    public async Task DisposeAsync()
    {
        _anonymousClient.Dispose();
        _adminClient.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task GET_PaymentMethods_WithoutToken_Returns401()
    {
        var response = await _anonymousClient.GetAsync("/api/payment-methods");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_PaymentMethodById_WithoutToken_Returns401()
    {
        var response = await _anonymousClient.GetAsync($"/api/payment-methods/{_methodId}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_PublicPaymentMethods_WithoutToken_Returns200()
    {
        var response = await _anonymousClient.GetAsync("/api/payment-methods/public");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("success").GetBoolean());
    }

    // The whole point of the fix: the storefront route must not carry the secret.
    // Asserted on the raw response text so a renamed or re-nested field cannot
    // slip the marker through a property-name check.
    [Fact]
    public async Task GET_PublicPaymentMethods_NeverExposesGatewayConfig()
    {
        var response = await _anonymousClient.GetAsync("/api/payment-methods/public");
        var raw = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain(SecretMarker, raw, StringComparison.Ordinal);
        Assert.DoesNotContain("gatewayConfig", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("webhookSecret", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("some-processor", raw, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GET_PublicPaymentMethods_ReturnsPublishableFields()
    {
        var response = await _anonymousClient.GetAsync("/api/payment-methods/public");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        var item = body.GetProperty("data").GetProperty("items")
            .EnumerateArray()
            .Single(x => x.GetProperty("id").GetInt64() == _methodId);

        Assert.Equal("Card", item.GetProperty("labelEn").GetString());
        Assert.Equal("online", item.GetProperty("type").GetString());
    }

    // Inactive methods are usually the ones still being configured, so their
    // gateway settings are the most sensitive rows in the table.
    [Fact]
    public async Task GET_PublicPaymentMethods_OmitsInactiveMethods()
    {
        long inactiveId;
        await using (var context = _fixture.CreateContext())
        {
            var inactive = new PaymentMethod
            {
                Name = $"inactive_{Guid.NewGuid():N}",
                LabelEn = "Sandbox",
                Type = "online",
                GatewayConfig = $$"""{"apiKey":"{{SecretMarker}}"}""",
                IsActive = false,
                SortOrder = 99,
                CreatedAt = DateTime.Now,
            };
            context.PaymentMethods.Add(inactive);
            await context.SaveChangesAsync();
            inactiveId = inactive.Id;
        }

        var response = await _anonymousClient.GetAsync("/api/payment-methods/public");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        var ids = body.GetProperty("data").GetProperty("items")
            .EnumerateArray()
            .Select(x => x.GetProperty("id").GetInt64())
            .ToList();

        Assert.DoesNotContain(inactiveId, ids);
    }

    // The admin panel still needs the field to edit it — the fix scopes the
    // exposure to authenticated callers, it does not delete the capability.
    [Fact]
    public async Task GET_PaymentMethodById_WithAdminToken_StillReturnsGatewayConfig()
    {
        var response = await _adminClient.GetAsync($"/api/payment-methods/{_methodId}");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            SecretMarker,
            body.GetProperty("data").GetProperty("gatewayConfig").GetString()!,
            StringComparison.Ordinal);
    }
}
