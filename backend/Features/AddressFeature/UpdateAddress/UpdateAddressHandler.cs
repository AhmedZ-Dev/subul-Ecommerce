using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AddressFeature.UpdateAddress;

public class UpdateAddressHandler(AppDbContext context)
    : IRequestHandler<UpdateAddressCommand, Result<UpdateAddressResponse>>
{
    public async Task<Result<UpdateAddressResponse>> Handle(
        UpdateAddressCommand command,
        CancellationToken cancellationToken)
    {
        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (address is null)
            return Result<UpdateAddressResponse>.Failure("Address not found");

        var isDefault = command.IsDefault;

        if (isDefault)
        {
            var otherDefaults = await context.Addresses
                .Where(a => a.UserId == address.UserId && a.Id != address.Id && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var addr in otherDefaults)
            {
                addr.IsDefault = false;
            }
        }
        else if (address.IsDefault)
        {
            // If trying to set default to false, check if this is the user's only address
            var hasOtherAddresses = await context.Addresses
                .AnyAsync(a => a.UserId == address.UserId && a.Id != address.Id, cancellationToken);

            if (!hasOtherAddresses)
            {
                isDefault = true; // force keep default since it is the only one
            }
        }

        address.FirstName = command.FirstName?.Trim();
        address.LastName = command.LastName?.Trim();
        address.Phone = command.Phone?.Trim();
        address.Address1 = command.Address1.Trim();
        address.Address2 = command.Address2?.Trim();
        address.City = command.City?.Trim();
        address.Governorate = command.Governorate?.Trim();
        address.Country = string.IsNullOrWhiteSpace(command.Country) ? "Iraq" : command.Country.Trim();
        address.IsDefault = isDefault;
        address.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync(cancellationToken);

        var response = new UpdateAddressResponse(
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

        return Result<UpdateAddressResponse>.Success(response);
    }
}
