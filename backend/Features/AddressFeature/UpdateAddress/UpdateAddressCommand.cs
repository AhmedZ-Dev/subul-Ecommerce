using System;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AddressFeature.UpdateAddress;

public record UpdateAddressCommand(
    long Id,
    string? FirstName,
    string? LastName,
    string? Phone,
    string Address1,
    string? Address2,
    string? City,
    string? Governorate,
    string Country,
    bool IsDefault) : IRequest<Result<UpdateAddressResponse>>;

public record UpdateAddressRequest(
    string? FirstName,
    string? LastName,
    string? Phone,
    string Address1,
    string? Address2,
    string? City,
    string? Governorate,
    string Country,
    bool IsDefault);

public record UpdateAddressResponse(
    long Id,
    long UserId,
    string? FirstName,
    string? LastName,
    string? Phone,
    string Address1,
    string? Address2,
    string? City,
    string? Governorate,
    string Country,
    bool IsDefault,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
