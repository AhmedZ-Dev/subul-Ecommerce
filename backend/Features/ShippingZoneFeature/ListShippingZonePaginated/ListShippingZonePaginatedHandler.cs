using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.ShippingZoneFeature.ListShippingZonePaginated;

public class ListShippingZonePaginatedHandler(AppDbContext context)
    : IRequestHandler<ListShippingZonePaginatedQuery, Result<ListShippingZonePaginatedResponse>>
{
    public async Task<Result<ListShippingZonePaginatedResponse>> Handle(
        ListShippingZonePaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var zonesQuery = context.ShippingZones.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            zonesQuery = zonesQuery.Where(z =>
                z.NameEn.ToLower().Contains(search) ||
                (z.NameAr != null && z.NameAr.ToLower().Contains(search)) ||
                (z.Governorates != null && z.Governorates.ToLower().Contains(search)));
        }

        if (query.IsActive is not null)
            zonesQuery = zonesQuery.Where(z => z.IsActive == query.IsActive);

        zonesQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("nameen", "asc") => zonesQuery.OrderBy(z => z.NameEn),
            ("nameen", "desc") => zonesQuery.OrderByDescending(z => z.NameEn),
            ("createdat", "asc") => zonesQuery.OrderBy(z => z.CreatedAt),
            _ => zonesQuery.OrderByDescending(z => z.CreatedAt)
        };

        var total = await zonesQuery.CountAsync(cancellationToken);

        var rawItems = await zonesQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(z => new
            {
                z.Id,
                z.NameEn,
                z.NameAr,
                z.Governorates,
                z.IsActive,
                z.CreatedAt,
                ShippingRateCount = z.ShippingRates.Count
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(z => new ListShippingZonePaginatedItemResponse(
                z.Id,
                z.NameEn,
                z.NameAr,
                ShippingZoneGovernorates.Parse(z.Governorates),
                z.IsActive,
                z.CreatedAt,
                z.ShippingRateCount)).ToList();

        var response = new ListShippingZonePaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListShippingZonePaginatedResponse>.Success(response);
    }
}
