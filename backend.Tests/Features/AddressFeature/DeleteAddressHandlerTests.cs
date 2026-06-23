using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AddressFeature.DeleteAddress;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests.Features.AddressFeature;

[Collection("Database")]
public class DeleteAddressHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_DeletesDefaultAddress_PromotesAnotherAddressAsDefault()
    {
        await using var context = fixture.CreateContext();
        var user = await CreateTestUserAsync(context);

        var defaultAddr = await CreateAddressAsync(context, user.Id, "Default Addr", true);
        var secondAddr = await CreateAddressAsync(context, user.Id, "Second Addr", false);

        var handler = new DeleteAddressHandler(context);
        var command = new DeleteAddressCommand(defaultAddr.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);

        // Check defaultAddr is deleted
        var exists = await context.Addresses.AnyAsync(a => a.Id == defaultAddr.Id);
        Assert.False(exists);

        // Check secondAddr is promoted to default
        var promoted = await context.Addresses.FirstAsync(a => a.Id == secondAddr.Id);
        Assert.True(promoted.IsDefault);
    }
}
