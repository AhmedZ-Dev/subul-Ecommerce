using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Domain.Entities;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ShippingZoneFeature.CreateShippingZone;

public class CreateShippingZoneHandler(AppDbContext context)
    : IRequestHandler<CreateShippingZoneCommand, Result<CreateShippingZoneResponse>>
{
    private static readonly HashSet<string> ValidRateTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "flat", "weight_based", "price_based"
    };

    public async Task<Result<CreateShippingZoneResponse>> Handle(
        CreateShippingZoneCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedNameEn = command.NameEn.Trim();

        var zoneExists = await context.ShippingZones.AnyAsync(
            sz => sz.NameEn.ToLower() == normalizedNameEn.ToLower(),
            cancellationToken);

        if (zoneExists)
            return Result<CreateShippingZoneResponse>.Failure("Shipping zone name already exists");

        var governoratesJson = command.Governorates is not null
            ? JsonSerializer.Serialize(command.Governorates)
            : null;

        var now = DateTime.Now;

        var zone = new ShippingZone
        {
            NameEn = normalizedNameEn,
            NameAr = command.NameAr?.Trim(),
            Governorates = governoratesJson,
            IsActive = command.IsActive,
            CreatedAt = now
        };

        if (command.ShippingRates is not null && command.ShippingRates.Count > 0)
        {
            foreach (var rateInput in command.ShippingRates)
            {
                var rateTypeNormalized = rateInput.RateType.Trim().ToLowerInvariant();
                if (!ValidRateTypes.Contains(rateTypeNormalized))
                    return Result<CreateShippingZoneResponse>.Failure($"Invalid shipping rate type: '{rateInput.RateType}'");

                zone.ShippingRates.Add(new ShippingRate
                {
                    NameEn = rateInput.NameEn?.Trim(),
                    NameAr = rateInput.NameAr?.Trim(),
                    RateType = rateTypeNormalized,
                    Price = rateInput.Price,
                    MinOrderValue = rateInput.MinOrderValue,
                    MaxOrderValue = rateInput.MaxOrderValue,
                    FreeShippingThreshold = rateInput.FreeShippingThreshold,
                    EstimatedDaysMin = rateInput.EstimatedDaysMin,
                    EstimatedDaysMax = rateInput.EstimatedDaysMax,
                    IsActive = rateInput.IsActive,
                    CreatedAt = now
                });
            }
        }

        context.ShippingZones.Add(zone);
        await context.SaveChangesAsync(cancellationToken);

        var mappedRates = zone.ShippingRates.Select(r => new CreateShippingRateResponse(
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

        var governoratesList = command.Governorates ?? new List<string>();

        var response = new CreateShippingZoneResponse(
            zone.Id,
            zone.NameEn,
            zone.NameAr,
            governoratesList,
            zone.IsActive,
            zone.CreatedAt,
            mappedRates);

        return Result<CreateShippingZoneResponse>.Success(response);
    }
}
