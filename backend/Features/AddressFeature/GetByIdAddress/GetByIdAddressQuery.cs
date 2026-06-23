using System;
using backend.Common.Results;
using MediatR;

namespace backend.Features.AddressFeature.GetByIdAddress;

public record GetByIdAddressQuery(long Id) : IRequest<Result<GetByIdAddressResponse>>;

public record GetByIdAddressResponse(
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
