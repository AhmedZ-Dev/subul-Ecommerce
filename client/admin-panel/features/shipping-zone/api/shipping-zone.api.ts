import apiClient from '@/lib/api-client';
import type {
  ShippingRateInfo,
  ShippingRateType,
  ShippingZoneDto,
  ShippingZoneListItem,
  ShippingZoneQueryParams,
} from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendShippingRate {
  id: number;
  shippingZoneId: number;
  nameEn: string | null;
  nameAr: string | null;
  rateType: string;
  price: number;
  minOrderValue: number | null;
  maxOrderValue: number | null;
  freeShippingThreshold: number | null;
  estimatedDaysMin: number | null;
  estimatedDaysMax: number | null;
  isActive: boolean;
  createdAt: string;
}

interface BackendShippingZoneListItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  governorates: string[];
  isActive: boolean;
  createdAt: string;
  shippingRateCount: number;
}

interface BackendShippingZoneDetail extends BackendShippingZoneListItem {
  shippingRates: BackendShippingRate[];
}

function toRateInfo(raw: BackendShippingRate): ShippingRateInfo {
  return {
    id: raw.id,
    shippingZoneId: raw.shippingZoneId,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    rateType: raw.rateType as ShippingRateType,
    price: raw.price,
    minOrderValue: raw.minOrderValue,
    maxOrderValue: raw.maxOrderValue,
    freeShippingThreshold: raw.freeShippingThreshold,
    estimatedDaysMin: raw.estimatedDaysMin,
    estimatedDaysMax: raw.estimatedDaysMax,
    isActive: raw.isActive,
    createdAt: raw.createdAt,
  };
}

function toListItem(raw: BackendShippingZoneListItem): ShippingZoneListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    governorates: raw.governorates ?? [],
    status: raw.isActive ? 'active' : 'inactive',
    shippingRateCount: raw.shippingRateCount,
    createdAt: raw.createdAt,
  };
}

function toDto(raw: BackendShippingZoneDetail): ShippingZoneDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    governorates: raw.governorates ?? [],
    status: raw.isActive ? 'active' : 'inactive',
    createdAt: raw.createdAt,
    shippingRates: (raw.shippingRates ?? []).map(toRateInfo),
  };
}

export interface ShippingRatePayload {
  id?: number;
  nameEn?: string;
  nameAr?: string;
  rateType: ShippingRateType;
  price: number;
  minOrderValue?: number | null;
  maxOrderValue?: number | null;
  freeShippingThreshold?: number | null;
  estimatedDaysMin?: number | null;
  estimatedDaysMax?: number | null;
  isActive: boolean;
}

export interface CreateShippingZonePayload {
  nameEn: string;
  nameAr?: string;
  governorates?: string[];
  isActive?: boolean;
  shippingRates?: ShippingRatePayload[];
}

export interface UpdateShippingZonePayload {
  nameEn: string;
  nameAr?: string;
  governorates?: string[];
  isActive: boolean;
  shippingRates?: ShippingRatePayload[];
}

function mapRatePayload(rate: ShippingRatePayload) {
  return {
    ...(rate.id !== undefined && rate.id > 0 ? { id: rate.id } : {}),
    nameEn: rate.nameEn ?? null,
    nameAr: rate.nameAr ?? null,
    rateType: rate.rateType,
    price: rate.price,
    minOrderValue: rate.minOrderValue ?? null,
    maxOrderValue: rate.maxOrderValue ?? null,
    freeShippingThreshold: rate.freeShippingThreshold ?? null,
    estimatedDaysMin: rate.estimatedDaysMin ?? null,
    estimatedDaysMax: rate.estimatedDaysMax ?? null,
    isActive: rate.isActive,
  };
}

export async function createShippingZone(
  payload: CreateShippingZonePayload,
): Promise<ShippingZoneDto> {
  const { data } = await apiClient.post<ApiResponse<BackendShippingZoneDetail>>('/shipping-zones', {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    governorates: payload.governorates ?? [],
    isActive: payload.isActive ?? true,
    shippingRates: (payload.shippingRates ?? []).map(mapRatePayload),
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to create shipping zone');
  return toDto(data.data!);
}

export async function updateShippingZone(
  id: number,
  payload: UpdateShippingZonePayload,
): Promise<ShippingZoneDto> {
  const { data } = await apiClient.put<ApiResponse<BackendShippingZoneDetail>>(
    `/shipping-zones/${id}`,
    {
      nameEn: payload.nameEn,
      nameAr: payload.nameAr ?? null,
      governorates: payload.governorates ?? [],
      isActive: payload.isActive,
      shippingRates: (payload.shippingRates ?? []).map(mapRatePayload),
    },
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update shipping zone');
  return toDto(data.data!);
}

export async function deleteShippingZone(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/shipping-zones/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete shipping zone');
}

export async function getShippingZoneById(id: number): Promise<ShippingZoneDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendShippingZoneDetail>>(
    `/shipping-zones/${id}`,
  );
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function getShippingZones(
  params: ShippingZoneQueryParams = {},
): Promise<PaginatedResponse<ShippingZoneListItem>> {
  const { page = 1, limit = 10, search, status, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendShippingZoneListItem[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >('/shipping-zones', {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(status !== undefined && { isActive: status === 'active' }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch shipping zones');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
