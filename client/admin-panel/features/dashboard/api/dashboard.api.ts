import apiClient from '@/lib/api-client';
import type { ApiResponse } from '@/types/api';
import type {
  DashboardStatsDto,
  LowStockProductItemDto,
  OrderStatsDto,
  OrdersByDayItemDto,
  ProductStatsDto,
  RecentOrderItemDto,
  TopSellingProductItemDto,
} from '../types';

interface BackendOrderStats {
  total: number;
  pending: number;
  processing: number;
  shipped: number;
  delivered: number;
  cancelled: number;
  paid: number;
  unpaid: number;
  totalRevenue: number;
  revenueToday: number;
  revenueThisMonth: number;
}

interface BackendProductStats {
  total: number;
  active: number;
  outOfStock: number;
  lowStock: number;
}

interface BackendRecentOrderItem {
  id: number;
  orderNumber: string;
  total: number;
  currency: string;
  status: string;
  paymentStatus: string;
  shippingFirstName: string | null;
  createdAt: string;
}

interface BackendOrdersByDayItem {
  date: string;
  orderCount: number;
  revenue: number;
}

interface BackendLowStockProductItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  stockQuantity: number;
  lowStockThreshold: number;
  primaryImageUrl: string | null;
}

interface BackendTopSellingProductItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  totalSold: number;
  price: number;
  currency: string;
  primaryImageUrl: string | null;
}

interface BackendDashboardStats {
  orders: BackendOrderStats;
  products: BackendProductStats;
  recentOrders: BackendRecentOrderItem[];
  ordersByDay: BackendOrdersByDayItem[];
  lowStockProducts: BackendLowStockProductItem[];
  topSellingProducts: BackendTopSellingProductItem[];
}

function toOrderStats(raw: BackendOrderStats): OrderStatsDto {
  return {
    total: raw.total,
    pending: raw.pending,
    processing: raw.processing,
    shipped: raw.shipped,
    delivered: raw.delivered,
    cancelled: raw.cancelled,
    paid: raw.paid,
    unpaid: raw.unpaid,
    totalRevenue: raw.totalRevenue,
    revenueToday: raw.revenueToday,
    revenueThisMonth: raw.revenueThisMonth,
  };
}

function toProductStats(raw: BackendProductStats): ProductStatsDto {
  return {
    total: raw.total,
    active: raw.active,
    outOfStock: raw.outOfStock,
    lowStock: raw.lowStock,
  };
}

function toRecentOrder(raw: BackendRecentOrderItem): RecentOrderItemDto {
  return {
    id: raw.id,
    orderNumber: raw.orderNumber,
    total: raw.total,
    currency: raw.currency,
    status: raw.status,
    paymentStatus: raw.paymentStatus,
    shippingFirstName: raw.shippingFirstName,
    createdAt: raw.createdAt,
  };
}

function toOrdersByDay(raw: BackendOrdersByDayItem): OrdersByDayItemDto {
  return {
    date: raw.date,
    orderCount: raw.orderCount,
    revenue: raw.revenue,
  };
}

function toLowStockProduct(raw: BackendLowStockProductItem): LowStockProductItemDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    stockQuantity: raw.stockQuantity,
    lowStockThreshold: raw.lowStockThreshold,
    primaryImageUrl: raw.primaryImageUrl,
  };
}

function toTopSellingProduct(raw: BackendTopSellingProductItem): TopSellingProductItemDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    totalSold: raw.totalSold,
    price: raw.price,
    currency: raw.currency,
    primaryImageUrl: raw.primaryImageUrl,
  };
}

function toDto(raw: BackendDashboardStats): DashboardStatsDto {
  return {
    orders: toOrderStats(raw.orders),
    products: toProductStats(raw.products),
    recentOrders: raw.recentOrders.map(toRecentOrder),
    ordersByDay: raw.ordersByDay.map(toOrdersByDay),
    lowStockProducts: raw.lowStockProducts.map(toLowStockProduct),
    topSellingProducts: raw.topSellingProducts.map(toTopSellingProduct),
  };
}

export async function getDashboardStats(): Promise<DashboardStatsDto> {
  const { data } = await apiClient.get<ApiResponse<BackendDashboardStats>>('/dashboard');
  if (!data.success) throw new Error(data.message ?? 'Failed to fetch dashboard stats');
  return toDto(data.data!);
}
