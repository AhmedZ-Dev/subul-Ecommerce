using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.ShippingZoneFeature.CreateShippingZone;

public record CreateShippingRateInput(
    string? NameEn,
    string? NameAr = null,
    string RateType = "flat", // flat / weight_based / price_based
    decimal Price = 0,
    decimal? MinOrderValue = null,
    decimal? MaxOrderValue = null,
    decimal? FreeShippingThreshold = null,
    int? EstimatedDaysMin = null,
    int? EstimatedDaysMax = null,
    bool IsActive = true);

public record CreateShippingZoneCommand(
    string NameEn,
    string? NameAr = null,
    List<string>? Governorates = null,
    bool IsActive = true,
    List<CreateShippingRateInput>? ShippingRates = null) : IRequest<Result<CreateShippingZoneResponse>>;

public record CreateShippingZoneResponse(
    long Id,
    string NameEn,
    string? NameAr,
    List<string> Governorates,
    bool IsActive,
    DateTime CreatedAt,
    List<CreateShippingRateResponse> ShippingRates);

public record CreateShippingRateResponse(
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
