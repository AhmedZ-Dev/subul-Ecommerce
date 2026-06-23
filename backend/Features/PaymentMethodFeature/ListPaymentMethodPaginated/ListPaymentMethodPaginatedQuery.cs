using backend.Common.Results;
using backend.Features.PaymentMethodFeature.CreatePaymentMethod;
using MediatR;

namespace backend.Features.PaymentMethodFeature.ListPaymentMethodPaginated;

public record ListPaymentMethodPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    string? Type = null,
    bool? IsActive = null,
    string? SortBy = "sortOrder",
    string? SortOrder = "asc") : IRequest<Result<ListPaymentMethodPaginatedResponse>>;

public record ListPaymentMethodPaginatedResponse(
    List<PaymentMethodResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);
