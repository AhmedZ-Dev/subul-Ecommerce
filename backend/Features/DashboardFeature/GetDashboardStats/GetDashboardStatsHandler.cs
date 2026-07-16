using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Common.Results;
using backend.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.DashboardFeature.GetDashboardStats;

public class GetDashboardStatsHandler(AppDbContext context)
    : IRequestHandler<GetDashboardStatsQuery, Result<GetDashboardStatsResponse>>
{
    public async Task<Result<GetDashboardStatsResponse>> Handle(
        GetDashboardStatsQuery query,
        CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var today = now.Date;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var thirtyDaysAgo = today.AddDays(-29);

        var orderStats = await LoadOrderStatsAsync(today, thisMonthStart, cancellationToken);
        var productStats = await LoadProductStatsAsync(cancellationToken);
        var recentOrders = await LoadRecentOrdersAsync(cancellationToken);
        var ordersByDay = await LoadOrdersByDayAsync(thirtyDaysAgo, today, cancellationToken);
        var lowStock = await LoadLowStockProductsAsync(cancellationToken);
        var topSelling = await LoadTopSellingProductsAsync(cancellationToken);

        var response = new GetDashboardStatsResponse(
            orderStats,
            productStats,
            recentOrders,
            ordersByDay,
            lowStock,
            topSelling);

        return Result<GetDashboardStatsResponse>.Success(response);
    }

    private async Task<OrderStats> LoadOrderStatsAsync(
        DateTime today,
        DateTime thisMonthStart,
        CancellationToken cancellationToken)
    {
        var stats = await context.Orders
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Pending = g.Count(o => o.Status == "pending"),
                Processing = g.Count(o => o.Status == "processing" || o.Status == "confirmed"),
                Shipped = g.Count(o => o.Status == "shipped" || o.Status == "out_for_delivery"),
                Delivered = g.Count(o => o.Status == "delivered"),
                Cancelled = g.Count(o => o.Status == "cancelled" || o.Status == "refunded"),
                Paid = g.Count(o => o.PaymentStatus == "paid"),
                Unpaid = g.Count(o => o.PaymentStatus == "pending"),
                TotalRevenue = g.Where(o => o.PaymentStatus == "paid").Sum(o => (decimal?)o.Total) ?? 0m,
                RevenueToday = g.Where(o => o.PaymentStatus == "paid" && o.CreatedAt >= today)
                    .Sum(o => (decimal?)o.Total) ?? 0m,
                RevenueThisMonth = g.Where(o => o.PaymentStatus == "paid" && o.CreatedAt >= thisMonthStart)
                    .Sum(o => (decimal?)o.Total) ?? 0m,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (stats is null)
        {
            return new OrderStats(0, 0, 0, 0, 0, 0, 0, 0, 0m, 0m, 0m);
        }

        return new OrderStats(
            stats.Total,
            stats.Pending,
            stats.Processing,
            stats.Shipped,
            stats.Delivered,
            stats.Cancelled,
            stats.Paid,
            stats.Unpaid,
            stats.TotalRevenue,
            stats.RevenueToday,
            stats.RevenueThisMonth);
    }

    private async Task<ProductStats> LoadProductStatsAsync(CancellationToken cancellationToken)
    {
        var stats = await context.Products
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Active = g.Count(p => p.Status == "active"),
                OutOfStock = g.Count(p => p.StockQuantity <= 0),
                LowStock = g.Count(p => p.StockQuantity > 0 && p.StockQuantity <= p.LowStockThreshold),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (stats is null)
        {
            return new ProductStats(0, 0, 0, 0);
        }

        return new ProductStats(stats.Total, stats.Active, stats.OutOfStock, stats.LowStock);
    }

    private Task<List<RecentOrderItem>> LoadRecentOrdersAsync(CancellationToken cancellationToken)
    {
        return context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new RecentOrderItem(
                o.Id,
                o.OrderNumber,
                o.Total,
                o.Currency,
                o.Status,
                o.PaymentStatus,
                o.ShippingFirstName,
                o.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<OrdersByDayItem>> LoadOrdersByDayAsync(
        DateTime thirtyDaysAgo,
        DateTime today,
        CancellationToken cancellationToken)
    {
        var raw = await context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedAt >= thirtyDaysAgo)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                OrderCount = g.Count(),
                Revenue = g.Where(o => o.PaymentStatus == "paid").Sum(o => (decimal?)o.Total) ?? 0m,
            })
            .ToListAsync(cancellationToken);

        var byDate = raw.ToDictionary(x => x.Date, x => x);
        var items = new List<OrdersByDayItem>();

        for (var date = thirtyDaysAgo; date <= today; date = date.AddDays(1))
        {
            if (byDate.TryGetValue(date, out var match))
            {
                items.Add(new OrdersByDayItem(
                    DateOnly.FromDateTime(match.Date),
                    match.OrderCount,
                    match.Revenue));
            }
            else
            {
                items.Add(new OrdersByDayItem(DateOnly.FromDateTime(date), 0, 0m));
            }
        }

        return items;
    }

    private Task<List<LowStockProductItem>> LoadLowStockProductsAsync(CancellationToken cancellationToken)
    {
        return context.Products
            .AsNoTracking()
            .Where(p => p.StockQuantity > 0 && p.StockQuantity <= p.LowStockThreshold)
            .OrderBy(p => p.StockQuantity)
            .ThenBy(p => p.NameEn)
            .Take(5)
            .Select(p => new LowStockProductItem(
                p.Id,
                p.NameEn,
                p.NameAr,
                p.StockQuantity,
                p.LowStockThreshold,
                context.ProductImages
                    .Where(pi => pi.ProductId == p.Id)
                    .OrderByDescending(pi => pi.IsPrimary)
                    .ThenBy(pi => pi.SortOrder)
                    .Select(pi => pi.ImageUrl)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }

    private Task<List<TopSellingProductItem>> LoadTopSellingProductsAsync(CancellationToken cancellationToken)
    {
        return context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.TotalSold)
            .ThenBy(p => p.NameEn)
            .Take(5)
            .Select(p => new TopSellingProductItem(
                p.Id,
                p.NameEn,
                p.NameAr,
                p.TotalSold,
                p.Price,
                p.Currency,
                context.ProductImages
                    .Where(pi => pi.ProductId == p.Id)
                    .OrderByDescending(pi => pi.IsPrimary)
                    .ThenBy(pi => pi.SortOrder)
                    .Select(pi => pi.ImageUrl)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }
}
