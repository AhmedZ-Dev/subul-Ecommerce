using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AddressFeature.CreateAddress;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AddressFeature;

[Collection("Database")]
public class CreateAddressHandlerTests(DatabaseFixture fixture)
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

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        await using var context = fixture.CreateContext();
        var user = await CreateTestUserAsync(context);

        var handler = new CreateAddressHandler(context);
        var command = new CreateAddressCommand(
            UserId: user.Id,
            FirstName: "Ahmed",
            LastName: "Z",
            Phone: "+9647700000000",
            Address1: "Karrada 601",
            Address2: "Near post office",
            City: "Baghdad",
            Governorate: "Baghdad"
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal("Karrada 601", result.Value.Address1);
        Assert.True(result.Value.IsDefault); // First address is auto default
    }

    [Fact]
    public async Task Handle_NonExistentUser_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new CreateAddressHandler(context);
        var command = new CreateAddressCommand(
            UserId: 999999,
            FirstName: "Ghost",
            LastName: "User",
            Phone: "123",
            Address1: "Street 1",
            Address2: null,
            City: "Erbil",
            Governorate: "Erbil"
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("User not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
