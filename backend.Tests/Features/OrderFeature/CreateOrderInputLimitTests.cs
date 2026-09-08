using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using backend.Domain.Entities;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.OrderFeature;

/// <summary>
/// Regression tests for the Wave B finding H-7, scoped to the only command an
/// unauthenticated caller can send.
///
/// POST api/orders writes into customer_notes, shipping_address1 and
/// shipping_address2 — all unbounded `text` columns, so Postgres accepted
/// whatever it was handed. With no validator and a request body limit sized for
/// image uploads, an anonymous caller could write megabytes per order.
///
/// Two independent limits are asserted here: the transport cap (Kestrel rejects
/// the body before it is read) and the validator (rejects the field after
/// binding). Neither substitutes for the other — the validator cannot prevent
/// the server from having already received a huge body.
/// </summary>
[Collection("Database")]
public class CreateOrderInputLimitTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private string _sessionId = null!;

    public CreateOrderInputLimitTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _client = _factory.CreateClient();
        _sessionId = Guid.NewGuid().ToString("N");

        // A cart with one item, so the request reaches validation on its merits
        // rather than bouncing off "Cart is empty".
        await using var context = _fixture.CreateContext();
        var now = DateTime.Now;

        var product = new Product
        {
            NameEn = $"Limit Test Product {Guid.NewGuid():N}",
            Slug = $"limit-test-{Guid.NewGuid():N}",
            Price = 1000,
            Currency = "IQD",
            Status = "active",
            StockQuantity = 50,
            CreatedAt = now,
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var cart = new Cart { SessionId = _sessionId, CreatedAt = now, UpdatedAt = now };
        context.Carts.Add(cart);
        await context.SaveChangesAsync();

        context.CartItems.Add(new CartItem
        {
            CartId = cart.Id,
            ProductId = product.Id,
            Quantity = 1,
            UnitPrice = product.Price,
            CreatedAt = now,
            UpdatedAt = now,
        });

        // CreateOrderHandler resolves shipping from the governorate and fails the
        // whole order without an active zone and flat rate, so the happy-path
        // assertions need one.
        var zone = new ShippingZone
        {
            NameEn = $"Baghdad Zone {Guid.NewGuid():N}",
            Governorates = "[\"بغداد\"]",
            IsActive = true,
            CreatedAt = now,
        };
        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync();

        context.ShippingRates.Add(new ShippingRate
        {
            ShippingZoneId = zone.Id,
            NameEn = "Flat",
            RateType = "flat",
            Price = 5000,
            IsActive = true,
            CreatedAt = now,
        });
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    private HttpRequestMessage BuildRequest(object payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"),
        };
        request.Headers.Add("X-Cart-Session", _sessionId);
        return request;
    }

    private static object ValidPayload(object? customerNotes = null, object? address1 = null) => new
    {
        shippingFirstName = "أحمد",
        shippingLastName = "زهير",
        shippingPhone = "07701234567",
        shippingAddress1 = address1 ?? "شارع الرشيد، بناية 12",
        shippingCity = "بغداد",
        shippingGovernorate = "بغداد",
        shippingCountry = "Iraq",
        paymentMethod = "cod",
        customerNotes,
    };

    // The field that had no limit anywhere: not in the DB, not in the API.
    [Fact]
    public async Task CustomerNotes_OverLimit_Returns400NotServerError()
    {
        var response = await _client.SendAsync(
            BuildRequest(ValidPayload(customerNotes: new string('ن', 501))));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CustomerNotes_OverLimit_ReturnsTheArabicFieldMessage()
    {
        var response = await _client.SendAsync(
            BuildRequest(ValidPayload(customerNotes: new string('ن', 501))));
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        var errors = body.GetProperty("errors").EnumerateArray()
            .Select(e => e.GetString() ?? string.Empty)
            .ToList();

        Assert.Contains(errors, e => e.Contains("الملاحظات", StringComparison.Ordinal));
    }

    [Fact]
    public async Task CustomerNotes_AtLimit_IsAccepted()
    {
        var response = await _client.SendAsync(
            BuildRequest(ValidPayload(customerNotes: new string('ن', 500))));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ShippingAddress_OverLimit_Returns400()
    {
        var response = await _client.SendAsync(
            BuildRequest(ValidPayload(address1: new string('ش', 256))));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ShippingPhone_WithInjectedText_Returns400()
    {
        var payload = new
        {
            shippingFirstName = "أحمد",
            shippingPhone = "<script>x</script>",
            shippingAddress1 = "شارع الرشيد",
            shippingCity = "بغداد",
            shippingGovernorate = "بغداد",
            paymentMethod = "cod",
        };

        var response = await _client.SendAsync(BuildRequest(payload));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // The transport cap (Kestrel MaxRequestBodySize) cannot be asserted here:
    // WebApplicationFactory runs on TestServer, which never applies
    // ConfigureKestrel, so an oversized body reaches the validator and comes back
    // 400 instead of 413. What this test can still prove is that a body far over
    // the cap is refused rather than written — the status code it is refused with
    // differs between TestServer and a real host.
    //
    // Verify the 413 against a running server:
    //   curl -s -o /dev/null -w '%{http_code}\n' -X POST localhost:5101/api/orders \
    //     -H 'Content-Type: application/json' -H 'X-Cart-Session: x' \
    //     --data-binary @<(python -c "print('{\"customerNotes\":\"' + 'x'*400000 + '\"}')")
    [Fact]
    public async Task OversizedBody_IsRefused()
    {
        var response = await _client.SendAsync(
            BuildRequest(ValidPayload(customerNotes: new string('ن', 400_000))));

        Assert.False(response.IsSuccessStatusCode);
    }

    // Nothing above should have made a legitimate order harder to place.
    [Fact]
    public async Task OrdinaryOrder_StillSucceeds()
    {
        var response = await _client.SendAsync(
            BuildRequest(ValidPayload(customerNotes: "يرجى الاتصال قبل التوصيل")));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
