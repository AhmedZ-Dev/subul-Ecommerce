using System;
using System.Threading;
using System.Threading.Tasks;
using backend.Domain.Entities;
using backend.Features.AddressFeature.GetByIdAddress;
using backend.Infrastructure.Persistence;
using backend.Tests.Infrastructure;
using Xunit;

namespace backend.Tests.Features.AddressFeature;

[Collection("Database")]
public class GetByIdAddressHandlerTests(DatabaseFixture fixture)
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
    public async Task Handle_ExistingId_ReturnsAddress()
    {
        await using var context = fixture.CreateContext();
        var user = await CreateTestUserAsync(context);

        var address = new Address
        {
            UserId = user.Id,
            FirstName = "Test",
            LastName = "User",
            Address1 = "Street ABC",
            City = "Basra",
            Governorate = "Basra",
            Country = "Iraq",
            IsDefault = true,
            CreatedAt = DateTime.Now
        };
        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        var handler = new GetByIdAddressHandler(context);
        var query = new GetByIdAddressQuery(address.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(address.Id, result.Value.Id);
        Assert.Equal("Street ABC", result.Value.Address1);
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsNotFound()
    {
        await using var context = fixture.CreateContext();
        var handler = new GetByIdAddressHandler(context);
        var query = new GetByIdAddressQuery(999999);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
