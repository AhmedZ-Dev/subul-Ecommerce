using System;
using System.Collections.Generic;
using backend.Common.Results;
using MediatR;

namespace backend.Features.ShippingZoneFeature.UpdateShippingZone;

public record UpdateShippingRateInput(
    long? Id,
    string? NameEn,
    string? NameAr = null,
    string RateType = "flat",
    decimal Price = 0,
    decimal? MinOrderValue = null,
    decimal? MaxOrderValue = null,
    decimal? FreeShippingThreshold = null,
    int? EstimatedDaysMin = null,
    int? EstimatedDaysMax = null,
    bool IsActive = true);

public record UpdateShippingZoneCommand(
    long Id,
    string NameEn,
    string? NameAr,
    List<string>? Governorates,
    bool IsActive,
    List<UpdateShippingRateInput>? ShippingRates) : IRequest<Result<UpdateShippingZoneResponse>>;

public record UpdateShippingZoneRequest(
    string NameEn,
    string? NameAr,
    List<string>? Governorates,
    bool IsActive,
    List<UpdateShippingRateInput>? ShippingRates);

public record UpdateShippingZoneResponse(
    long Id,
    string NameEn,
    string? NameAr,
    List<string> Governorates,
    bool IsActive,
    DateTime CreatedAt,
    List<UpdateShippingRateResponse> ShippingRates);

public record UpdateShippingRateResponse(
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
