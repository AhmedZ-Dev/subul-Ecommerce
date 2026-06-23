using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ShippingZoneFeature.GetByIdShippingZone;

public class GetByIdShippingZoneHandler(AppDbContext context)
    : IRequestHandler<GetByIdShippingZoneQuery, Result<GetByIdShippingZoneResponse>>
{
    public async Task<Result<GetByIdShippingZoneResponse>> Handle(
        GetByIdShippingZoneQuery query,
        CancellationToken cancellationToken)
    {
        var zone = await context.ShippingZones
            .AsNoTracking()
            .Include(z => z.ShippingRates)
            .FirstOrDefaultAsync(z => z.Id == query.Id, cancellationToken);

        if (zone is null)
            return Result<GetByIdShippingZoneResponse>.Failure("Shipping zone not found");

        var governorates = string.IsNullOrWhiteSpace(zone.Governorates)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(zone.Governorates) ?? new List<string>();

        var mappedRates = zone.ShippingRates.Select(r => new GetByIdShippingRateResponse(
            r.Id,
            r.ShippingZoneId,
            r.NameEn,
            r.NameAr,
            r.RateType,
            r.Price,
            r.MinOrderValue,
            r.MaxOrderValue,
            r.FreeShippingThreshold,
            r.EstimatedDaysMin,
            r.EstimatedDaysMax,
            r.IsActive,
            r.CreatedAt)).ToList();

        var response = new GetByIdShippingZoneResponse(
            zone.Id,
            zone.NameEn,
            zone.NameAr,
            governorates,
            zone.IsActive,
            zone.CreatedAt,
            mappedRates);

        return Result<GetByIdShippingZoneResponse>.Success(response);
    }
}
