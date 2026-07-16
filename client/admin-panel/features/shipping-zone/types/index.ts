export type ShippingZoneStatus = 'active' | 'inactive';
export type ShippingRateType = 'flat' | 'weight_based' | 'price_based';

export interface ShippingRateInfo {
  id: number;
  shippingZoneId: number;
  nameEn: string | null;
  nameAr: string | null;
  rateType: ShippingRateType;
  price: number;
  minOrderValue: number | null;
  maxOrderValue: number | null;
  freeShippingThreshold: number | null;
  estimatedDaysMin: number | null;
  estimatedDaysMax: number | null;
  isActive: boolean;
  createdAt: string;
}

export interface ShippingZoneDto {
  id: number;
  nameEn: string;
  nameAr: string | null;
  governorates: string[];
  status: ShippingZoneStatus;
  createdAt: string;
  shippingRates: ShippingRateInfo[];
}

export interface ShippingZoneListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  governorates: string[];
  status: ShippingZoneStatus;
  shippingRateCount: number;
  createdAt: string;
}

export interface ShippingZoneQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  status?: ShippingZoneStatus;
  sortBy?: 'nameEn' | 'createdAt';
  sortOrder?: 'asc' | 'desc';
}
