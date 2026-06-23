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

namespace backend.Features.ShippingZoneFeature.UpdateShippingZone;

public class UpdateShippingZoneHandler(AppDbContext context)
    : IRequestHandler<UpdateShippingZoneCommand, Result<UpdateShippingZoneResponse>>
{
    private static readonly HashSet<string> ValidRateTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "flat", "weight_based", "price_based"
    };

    public async Task<Result<UpdateShippingZoneResponse>> Handle(
        UpdateShippingZoneCommand command,
        CancellationToken cancellationToken)
    {
        var zone = await context.ShippingZones
            .Include(z => z.ShippingRates)
            .FirstOrDefaultAsync(z => z.Id == command.Id, cancellationToken);

        if (zone is null)
            return Result<UpdateShippingZoneResponse>.Failure("Shipping zone not found");

        var normalizedNameEn = command.NameEn.Trim();

        var zoneExists = await context.ShippingZones.AnyAsync(
            sz => sz.Id != command.Id && sz.NameEn.ToLower() == normalizedNameEn.ToLower(),
            cancellationToken);

        if (zoneExists)
            return Result<UpdateShippingZoneResponse>.Failure("Shipping zone name already exists");

        var governoratesJson = command.Governorates is not null
            ? JsonSerializer.Serialize(command.Governorates)
            : null;

        var now = DateTime.Now;

        zone.NameEn = normalizedNameEn;
        zone.NameAr = command.NameAr?.Trim();
        zone.Governorates = governoratesJson;
        zone.IsActive = command.IsActive;

        // Sync shipping rates
        var inputIds = command.ShippingRates?
            .Where(r => r.Id.HasValue && r.Id.Value > 0)
            .Select(r => r.Id!.Value)
            .ToList() ?? new List<long>();

        var toDeleteRates = zone.ShippingRates.Where(r => !inputIds.Contains(r.Id)).ToList();
        foreach (var rate in toDeleteRates)
        {
            context.ShippingRates.Remove(rate);
        }

        if (command.ShippingRates is not null)
        {
            foreach (var rateInput in command.ShippingRates)
            {
                var rateTypeNormalized = rateInput.RateType.Trim().ToLowerInvariant();
                if (!ValidRateTypes.Contains(rateTypeNormalized))
                    return Result<UpdateShippingZoneResponse>.Failure($"Invalid shipping rate type: '{rateInput.RateType}'");

                if (rateInput.Id.HasValue && rateInput.Id.Value > 0)
                {
                    var existingRate = zone.ShippingRates.FirstOrDefault(r => r.Id == rateInput.Id.Value);
                    if (existingRate is not null)
                    {
                        existingRate.NameEn = rateInput.NameEn?.Trim();
                        existingRate.NameAr = rateInput.NameAr?.Trim();
                        existingRate.RateType = rateTypeNormalized;
                        existingRate.Price = rateInput.Price;
                        existingRate.MinOrderValue = rateInput.MinOrderValue;
                        existingRate.MaxOrderValue = rateInput.MaxOrderValue;
                        existingRate.FreeShippingThreshold = rateInput.FreeShippingThreshold;
                        existingRate.EstimatedDaysMin = rateInput.EstimatedDaysMin;
                        existingRate.EstimatedDaysMax = rateInput.EstimatedDaysMax;
                        existingRate.IsActive = rateInput.IsActive;
                    }
                }
                else
                {
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
        }

        await context.SaveChangesAsync(cancellationToken);

        var mappedRates = zone.ShippingRates.Select(r => new UpdateShippingRateResponse(
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

        var response = new UpdateShippingZoneResponse(
            zone.Id,
            zone.NameEn,
            zone.NameAr,
            governoratesList,
            zone.IsActive,
            zone.CreatedAt,
            mappedRates);

        return Result<UpdateShippingZoneResponse>.Success(response);
    }
}
