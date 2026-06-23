using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.ShippingZoneFeature.GetByIdShippingZone;

public record GetByIdShippingZoneQuery(long Id) : IRequest<Result<GetByIdShippingZoneResponse>>;

public record GetByIdShippingZoneResponse(
    long Id,
    string NameEn,
    string? NameAr,
    List<string> Governorates,
    bool IsActive,
    DateTime CreatedAt,
    List<GetByIdShippingRateResponse> ShippingRates);

public record GetByIdShippingRateResponse(
    long Id,
    long ShippingZoneId,
    string? NameEn,
    string? NameAr,
    string RateType,
    decimal Price,
    decimal? MinOrderValue,
    decimal? MaxOrderValue,
    decimal? FreeShippingThreshold,
    int? EstimatedDaysMin,
    int? EstimatedDaysMax,
    bool IsActive,
    DateTime CreatedAt);
