using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.PaymentMethodFeature.ListPublicPaymentMethods;

public class ListPublicPaymentMethodsHandler(AppDbContext context)
    : IRequestHandler<ListPublicPaymentMethodsQuery, Result<ListPublicPaymentMethodsResponse>>
{
    public async Task<Result<ListPublicPaymentMethodsResponse>> Handle(
        ListPublicPaymentMethodsQuery query,
        CancellationToken cancellationToken)
    {
        // IsActive is filtered here, not taken from the caller: a disabled method
        // is usually one still being configured, and its gateway settings are the
        // most sensitive rows in the table.
        var items = await context.PaymentMethods
            .AsNoTracking()
            .Where(pm => pm.IsActive)
            .OrderBy(pm => pm.SortOrder)
            .ThenBy(pm => pm.Id)
            .Select(pm => new PublicPaymentMethodResponse(
                pm.Id,
                pm.Name,
                pm.LabelEn,
                pm.LabelAr,
                pm.Type,
                pm.IconUrl,
                pm.InstructionsEn,
                pm.InstructionsAr,
                pm.SortOrder))
            .ToListAsync(cancellationToken);

        return Result<ListPublicPaymentMethodsResponse>.Success(
            new ListPublicPaymentMethodsResponse(items));
    }
}
