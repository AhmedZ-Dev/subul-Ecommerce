using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.AddressFeature.ListAddressPaginated;

public class ListAddressPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListAddressPaginatedQuery, Result<ListAddressPaginatedResponse>>
{
    public async Task<Result<ListAddressPaginatedResponse>> Handle(
        ListAddressPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var addressQuery = context.Addresses.AsNoTracking().AsQueryable();

        if (query.UserId is not null)
            addressQuery = addressQuery.Where(a => a.UserId == query.UserId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            addressQuery = addressQuery.Where(a =>
                a.Address1.ToLower().Contains(search) ||
                (a.Address2 != null && a.Address2.ToLower().Contains(search)) ||
                (a.City != null && a.City.ToLower().Contains(search)) ||
                (a.Governorate != null && a.Governorate.ToLower().Contains(search)) ||
                (a.FirstName != null && a.FirstName.ToLower().Contains(search)) ||
                (a.LastName != null && a.LastName.ToLower().Contains(search)) ||
                (a.Phone != null && a.Phone.ToLower().Contains(search)));
        }

        if (query.IsDefault is not null)
            addressQuery = addressQuery.Where(a => a.IsDefault == query.IsDefault);

        addressQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("updatedat", "asc") => addressQuery.OrderBy(a => a.UpdatedAt),
            ("updatedat", "desc") => addressQuery.OrderByDescending(a => a.UpdatedAt),
            ("createdat", "asc") => addressQuery.OrderBy(a => a.CreatedAt),
            _ => addressQuery.OrderByDescending(a => a.CreatedAt)
        };

        var total = await addressQuery.CountAsync(cancellationToken);

        var rawItems = await addressQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(address => new ListAddressPaginatedItemResponse(
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
            address.UpdatedAt)).ToList();

        var response = new ListAddressPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListAddressPaginatedResponse>.Success(response);
    }
}
