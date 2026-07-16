using backend.Common.Results;
using MediatR;

namespace backend.Features.DashboardFeature.GetDashboardStats;

public record GetDashboardStatsQuery() : IRequest<Result<GetDashboardStatsResponse>>;

public record GetDashboardStatsResponse(
    OrderStats Orders,
    ProductStats Products,
    List<RecentOrderItem> RecentOrders,
    List<OrdersByDayItem> OrdersByDay,
    List<LowStockProductItem> LowStockProducts,
    List<TopSellingProductItem> TopSellingProducts);

public record OrderStats(
    int Total,
    int Pending,
    int Processing,
    int Shipped,
    int Delivered,
    int Cancelled,
    int Paid,
    int Unpaid,
    decimal TotalRevenue,
    decimal RevenueToday,
    decimal RevenueThisMonth);

public record ProductStats(
    int Total,
    int Active,
    int OutOfStock,
    int LowStock);

public record RecentOrderItem(
    long Id,
    string OrderNumber,
    decimal Total,
    string Currency,
    string Status,
    string PaymentStatus,
    string? ShippingFirstName,
    DateTime CreatedAt);

public record OrdersByDayItem(
    DateOnly Date,
    int OrderCount,
    decimal Revenue);

public record LowStockProductItem(
    long Id,
    string NameEn,
    string? NameAr,
    int StockQuantity,
    int LowStockThreshold,
    string? PrimaryImageUrl);

public record TopSellingProductItem(
    long Id,
    string NameEn,
    string? NameAr,
    int TotalSold,
    decimal Price,
    string Currency,
    string? PrimaryImageUrl);
