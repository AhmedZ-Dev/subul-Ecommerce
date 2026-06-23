using System;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AddressFeature.CreateAddress;

public record CreateAddressCommand(
    long UserId,
    string? FirstName,
    string? LastName,
    string? Phone,
    string Address1,
    string? Address2,
    string? City,
    string? Governorate,
    string Country = "Iraq",
    bool IsDefault = false) : IRequest<Result<CreateAddressResponse>>;

public record CreateAddressResponse(
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
