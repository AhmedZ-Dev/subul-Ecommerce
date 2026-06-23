using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AddressFeature.UpdateAddress;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace backend.Tests.Features.AddressFeature;

[Collection("Database")]
public class UpdateAddressHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ValidCommand_UpdatesFieldsAndResetsOtherDefaults()
    {
        await using var context = fixture.CreateContext();
        var user = await CreateTestUserAsync(context);

        var a1 = await CreateAddressAsync(context, user.Id, "Old Addr A", true);
        var a2 = await CreateAddressAsync(context, user.Id, "Old Addr B", false);

        var handler = new UpdateAddressHandler(context);
        var command = new UpdateAddressCommand(
            Id: a2.Id,
            FirstName: "New Name",
            LastName: "Last",
            Phone: "777",
            Address1: "New Road 10",
            Address2: null,
            City: "Basra",
            Governorate: "Basra",
            Country: "Iraq",
            IsDefault: true // Toggle default to true
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("New Road 10", result.Value.Address1);
        Assert.True(result.Value.IsDefault);

        // Check that other address is no longer default
        var otherAddress = await context.Addresses.FirstAsync(a => a.Id == a1.Id);
        Assert.False(otherAddress.IsDefault);
    }

    [Fact]
    public async Task Handle_ChangeDefaultToFalseOnOnlyAddress_KeepsItDefault()
    {
        await using var context = fixture.CreateContext();
        var user = await CreateTestUserAsync(context);
        var a1 = await CreateAddressAsync(context, user.Id, "Only Addr", true);

        var handler = new UpdateAddressHandler(context);
        var command = new UpdateAddressCommand(
            Id: a1.Id,
            FirstName: "Only",
            LastName: "One",
            Phone: "123",
            Address1: "Only Addr Updated",
            Address2: null,
            City: "Baghdad",
            Governorate: "Baghdad",
            Country: "Iraq",
            IsDefault: false // Trying to set false
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.IsDefault); // Still remains true since it is the only one
    }
}
