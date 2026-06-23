using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AddressFeature.ListAddressPaginated;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AddressFeature;

[Collection("Database")]
public class ListAddressHandlerTests(DatabaseFixture fixture)
{
    private async Task<User> CreateTestUserAsync(AppDbContext context)
    {
        var user = new User
        {
            Email = "user_" + Guid.NewGuid() + "@test.com",
            StoreCredit = 0m,
            CreatedAt = DateTime.Now
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task<Address> CreateAddressAsync(AppDbContext context, long userId, string address1, bool isDefault)
    {
        var address = new Address
        {
            UserId = userId,
            Address1 = address1,
            Country = "Iraq",
            IsDefault = isDefault,
            CreatedAt = DateTime.Now
        };
        context.Addresses.Add(address);
        await context.SaveChangesAsync();
        return address;
    }

    [Fact]
    public async Task Handle_FilterByUserId_ReturnsUserAddressesOnly()
    {
        await using var context = fixture.CreateContext();
        var u1 = await CreateTestUserAsync(context);
        var u2 = await CreateTestUserAsync(context);

        await CreateAddressAsync(context, u1.Id, "U1 Addr A", true);
        await CreateAddressAsync(context, u2.Id, "U2 Addr B", true);

        var handler = new ListAddressPaginatedHandler(context);
        var query = new ListAddressPaginatedQuery(UserId: u1.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("U1 Addr A", result.Value.Items[0].Address1);
    }

    [Fact]
    public async Task Handle_FilterBySearch_ReturnsMatchingOnly()
    {
        await using var context = fixture.CreateContext();
        var user = await CreateTestUserAsync(context);
        var searchKey = "SearchKey_" + Guid.NewGuid().ToString("N");

        await CreateAddressAsync(context, user.Id, "Road " + searchKey, true);
        await CreateAddressAsync(context, user.Id, "Normal street", false);

        var handler = new ListAddressPaginatedHandler(context);
        var query = new ListAddressPaginatedQuery(Search: searchKey);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Contains(searchKey, result.Value.Items[0].Address1);
    }
}
