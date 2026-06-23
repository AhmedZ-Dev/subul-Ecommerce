using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AddressFeature.DeleteAddress;

public class DeleteAddressHandler(AppDbContext context)
    : IRequestHandler<DeleteAddressCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteAddressCommand command,
        CancellationToken cancellationToken)
    {
        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (address is null)
            return Result<bool>.Failure("Address not found");

        if (address.IsDefault)
        {
            var nextDefault = await context.Addresses
                .Where(a => a.UserId == address.UserId && a.Id != address.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
            }
        }

        context.Addresses.Remove(address);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
