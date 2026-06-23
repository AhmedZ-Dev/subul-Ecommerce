using backend.Common.Results;
using backend.Features.OrderFeature.GetByIdOrder;
using MediatR;

namespace backend.Features.OrderFeature.ListOrderPaginated;

public record ListOrderPaginatedQuery(
    int Page = 1,
    int Limit = 10,
    string? Search = null,
    long? UserId = null,
    string? Status = null,
    string? PaymentStatus = null,
    string? FulfillmentStatus = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<Result<ListOrderPaginatedResponse>>;

public record ListOrderPaginatedResponse(
    List<ListOrderPaginatedItemResponse> Items,
    int Total,
    int Page,
    int Limit,
    int TotalPages);

public record ListOrderPaginatedItemResponse(
    long Id,
    string OrderNumber,
    long? UserId,
    string Status,
    string PaymentStatus,
    string FulfillmentStatus,
    decimal Total,
    string Currency,
    string? ShippingFirstName,
    string? ShippingPhone,
    string? ShippingCity,
    string? ShippingGovernorate,
    string? PaymentMethod,
    string? TrackingNumber,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int ItemCount,
    IReadOnlyList<OrderItemResponse> Items);
