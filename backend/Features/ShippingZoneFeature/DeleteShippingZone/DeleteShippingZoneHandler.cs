using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ShippingZoneFeature.DeleteShippingZone;

public class DeleteShippingZoneHandler(AppDbContext context)
    : IRequestHandler<DeleteShippingZoneCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteShippingZoneCommand command,
        CancellationToken cancellationToken)
    {
        var zone = await context.ShippingZones
            .Include(z => z.ShippingRates)
            .FirstOrDefaultAsync(z => z.Id == command.Id, cancellationToken);

        if (zone is null)
            return Result<bool>.Failure("Shipping zone not found");

        var hasOrders = await context.Orders.AnyAsync(o => o.ShippingZoneId == command.Id, cancellationToken);
        if (hasOrders)
            return Result<bool>.Failure("Cannot delete shipping zone because it is associated with orders");

        foreach (var rate in zone.ShippingRates)
        {
            context.ShippingRates.Remove(rate);
        }

        context.ShippingZones.Remove(zone);
        await context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
