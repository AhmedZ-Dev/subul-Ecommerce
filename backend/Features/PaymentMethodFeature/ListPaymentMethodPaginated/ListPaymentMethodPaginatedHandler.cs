using backend.Common.Results;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.ListPaymentMethodPaginated;

public class ListPaymentMethodPaginatedHandler(AppDbContext context)
    : IRequestHandler<ListPaymentMethodPaginatedQuery, Result<ListPaymentMethodPaginatedResponse>>
{
    public async Task<Result<ListPaymentMethodPaginatedResponse>> Handle(
        ListPaymentMethodPaginatedQuery query,
        CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var limit = query.Limit <= 0 ? 10 : query.Limit;

        var methodQuery = context.PaymentMethods.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            methodQuery = methodQuery.Where(pm =>
                pm.Name.ToLower().Contains(search) ||
                (pm.LabelEn != null && pm.LabelEn.ToLower().Contains(search)) ||
                (pm.LabelAr != null && pm.LabelAr.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            var type = query.Type.Trim().ToLowerInvariant();
            methodQuery = methodQuery.Where(pm => pm.Type != null && pm.Type.ToLower() == type);
        }

        if (query.IsActive is not null)
            methodQuery = methodQuery.Where(pm => pm.IsActive == query.IsActive);

        methodQuery = (query.SortBy?.ToLower(), query.SortOrder?.ToLower()) switch
        {
            ("name", "asc") => methodQuery.OrderBy(pm => pm.Name),
            ("name", "desc") => methodQuery.OrderByDescending(pm => pm.Name),
            ("createdat", "asc") => methodQuery.OrderBy(pm => pm.CreatedAt),
            ("createdat", "desc") => methodQuery.OrderByDescending(pm => pm.CreatedAt),
            ("sortorder", "desc") => methodQuery.OrderByDescending(pm => pm.SortOrder),
            _ => methodQuery.OrderBy(pm => pm.SortOrder).ThenBy(pm => pm.Name)
        };

        var total = await methodQuery.CountAsync(cancellationToken);

        var methods = await methodQuery
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var items = methods
            .Select(CreatePaymentMethodHandler.MapResponse)
            .ToList();

        var response = new ListPaymentMethodPaginatedResponse(
            items,
            total,
            page,
            limit,
            total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit));

        return Result<ListPaymentMethodPaginatedResponse>.Success(response);
    }
}
