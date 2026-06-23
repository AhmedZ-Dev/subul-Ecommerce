using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AddressFeature.GetByIdAddress;

public class GetByIdAddressHandler(AppDbContext context)
    : IRequestHandler<GetByIdAddressQuery, Result<GetByIdAddressResponse>>
{
    public async Task<Result<GetByIdAddressResponse>> Handle(
        GetByIdAddressQuery query,
        CancellationToken cancellationToken)
    {
        var address = await context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.Id, cancellationToken);

        if (address is null)
            return Result<GetByIdAddressResponse>.Failure("Address not found");

        var response = new GetByIdAddressResponse(
            address.Id,
            address.UserId,
            address.FirstName,
            address.LastName,
            address.Phone,
            address.Address1,
            address.Address2,
            address.City,
            address.Governorate,
            address.Country,
            address.IsDefault,
            address.CreatedAt,
            address.UpdatedAt);

        return Result<GetByIdAddressResponse>.Success(response);
    }
}
