using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AddressFeature.CreateAddress;

public class CreateAddressHandler(AppDbContext context)
    : IRequestHandler<CreateAddressCommand, Result<CreateAddressResponse>>
{
    public async Task<Result<CreateAddressResponse>> Handle(
        CreateAddressCommand command,
        CancellationToken cancellationToken)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken);
        if (!userExists)
            return Result<CreateAddressResponse>.Failure("User not found");

        var isDefault = command.IsDefault;

        // If the user has no address yet, make it the default address
        var hasAddresses = await context.Addresses.AnyAsync(a => a.UserId == command.UserId, cancellationToken);
        if (!hasAddresses)
        {
            isDefault = true;
        }

        if (isDefault)
        {
            var defaultAddresses = await context.Addresses
                .Where(a => a.UserId == command.UserId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var addr in defaultAddresses)
            {
                addr.IsDefault = false;
            }
        }

        var now = DateTime.Now;

        var address = new Address
        {
            UserId = command.UserId,
            FirstName = command.FirstName?.Trim(),
            LastName = command.LastName?.Trim(),
            Phone = command.Phone?.Trim(),
            Address1 = command.Address1.Trim(),
            Address2 = command.Address2?.Trim(),
            City = command.City?.Trim(),
            Governorate = command.Governorate?.Trim(),
            Country = string.IsNullOrWhiteSpace(command.Country) ? "Iraq" : command.Country.Trim(),
            IsDefault = isDefault,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(cancellationToken);

        var response = new CreateAddressResponse(
            address.Id,
            address.UserId,
            address.FirstName,
            address.LastName,
            address.Phone,
            address.Address1,
            address.Address2,
            address.City,
            address.Governorate,
            address.Country,
            address.IsDefault,
            address.CreatedAt,
            address.UpdatedAt);

        return Result<CreateAddressResponse>.Success(response);
    }
}
