using System.Net;
using System.Net.Http.Json;
using backend.Tests.Infrastructure;

namespace backend.Tests.Features.OrderFeature;

/// <summary>
/// Regression tests for the Wave A access-control findings (C-1, C-2, C-3, C-6).
/// Order reads and writes are admin-only; only guest checkout and guest tracking
/// stay anonymous. If any of these start returning 200 to <see cref="_anonymousClient"/>,
/// an [AllowAnonymous] has been reintroduced on an admin route.
/// </summary>
[Collection("Database")]
public class OrderAccessControlTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private TestWebApplicationFactory _factory = null!;
    private HttpClient _anonymousClient = null!;
    private HttpClient _adminClient = null!;

    public OrderAccessControlTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _factory = new TestWebApplicationFactory(_fixture.ConnectionString);
        _anonymousClient = _factory.CreateClient();
        await using var context = _fixture.CreateContext();
        _adminClient = await AuthTestHelper.CreateAuthenticatedClientAsync(_factory, context);
    }

    public async Task DisposeAsync()
    {
        _anonymousClient.Dispose();
        _adminClient.Dispose();
        await _factory.DisposeAsync();
    }

    // C-1: GET /api/orders dumped every order — name, phone, address, total — to anyone.
    [Fact]
    public async Task GET_Orders_WithoutToken_Returns401()
    {
        var response = await _anonymousClient.GetAsync("/api/orders");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // C-1: the search filter matched on ShippingPhone, so it doubled as a lookup by person.
    [Fact]
    public async Task GET_Orders_SearchByPhone_WithoutToken_Returns401()
    {
        var response = await _anonymousClient.GetAsync("/api/orders?search=07701234567&limit=999999");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // C-2: sequential long ids made every order individually enumerable.
    [Fact]
    public async Task GET_OrderById_WithoutToken_Returns401()
    {
        var response = await _anonymousClient.GetAsync("/api/orders/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_OrderItems_WithoutToken_Returns401()
    {
        var listResponse = await _anonymousClient.GetAsync("/api/orders/1/items");
        Assert.Equal(HttpStatusCode.Unauthorized, listResponse.StatusCode);

        var itemResponse = await _anonymousClient.GetAsync("/api/orders/1/items/1");
        Assert.Equal(HttpStatusCode.Unauthorized, itemResponse.StatusCode);
    }

    // C-3: anyone could mark an unpaid order paid, or cancel every order in a loop.
    [Fact]
    public async Task PUT_Order_WithoutToken_Returns401()
    {
        var response = await _anonymousClient.PutAsJsonAsync(
            "/api/orders/1",
            new { paymentStatus = "paid" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // The admin path must keep working — these routes are protected, not removed.
    [Fact]
    public async Task GET_Orders_WithAdminToken_Returns200()
    {
        var response = await _adminClient.GetAsync("/api/orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Guest checkout and guest tracking are the only order routes that stay anonymous.
    [Fact]
    public async Task GuestOrderRoutes_StayAnonymous()
    {
        var trackResponse = await _anonymousClient.GetAsync(
            "/api/orders/track?orderNumber=ORD-does-not-exist&phone=07700000000");
        Assert.NotEqual(HttpStatusCode.Unauthorized, trackResponse.StatusCode);

        using var checkoutRequest = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        checkoutRequest.Headers.Add("X-Cart-Session", Guid.NewGuid().ToString("N"));
        checkoutRequest.Content = JsonContent.Create(new { paymentMethod = "cod" });
        var checkoutResponse = await _anonymousClient.SendAsync(checkoutRequest);
        Assert.NotEqual(HttpStatusCode.Unauthorized, checkoutResponse.StatusCode);
    }

    // C-6: CreateOrder matched carts on a caller-supplied userId, so an attacker could
    // check out someone else's cart. The endpoint no longer binds userId at all —
    // sending it must not resolve another session's cart.
    [Fact]
    public async Task POST_Order_WithForeignUserId_DoesNotUseAnotherSessionsCart()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        request.Headers.Add("X-Cart-Session", Guid.NewGuid().ToString("N"));
        request.Content = JsonContent.Create(new
        {
            userId = 1,
            addressId = 1,
            paymentMethod = "cod",
            shippingFirstName = "Attacker",
            shippingPhone = "07700000000",
            shippingAddress1 = "somewhere",
            shippingCity = "Baghdad",
            shippingGovernorate = "Baghdad",
        });

        var response = await _anonymousClient.SendAsync(request);

        // The attacker's own session has no cart, so checkout fails on an empty cart
        // rather than succeeding against the victim's.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
