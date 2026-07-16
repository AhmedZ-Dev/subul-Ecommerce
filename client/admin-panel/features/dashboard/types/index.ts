export interface OrderStatsDto {
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

export interface ProductStatsDto {
  total: number;
  active: number;
  outOfStock: number;
  lowStock: number;
}

export interface RecentOrderItemDto {
  id: number;
  orderNumber: string;
  total: number;
  currency: string;
  status: string;
  paymentStatus: string;
  shippingFirstName: string | null;
  createdAt: string;
}

export interface OrdersByDayItemDto {
  date: string;
  orderCount: number;
  revenue: number;
}

export interface LowStockProductItemDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  stockQuantity: number;
  lowStockThreshold: number;
  primaryImageUrl: string | null;
}

export interface TopSellingProductItemDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  totalSold: number;
  price: number;
  currency: string;
  primaryImageUrl: string | null;
}

export interface DashboardStatsDto {
  orders: OrderStatsDto;
  products: ProductStatsDto;
  recentOrders: RecentOrderItemDto[];
  ordersByDay: OrdersByDayItemDto[];
  lowStockProducts: LowStockProductItemDto[];
  topSellingProducts: TopSellingProductItemDto[];
}
