import apiClient from '@/lib/api-client';
import type {
  PaymentMethodDto,
  PaymentMethodListItem,
  PaymentMethodQueryParams,
  PaymentMethodType,
} from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendPaymentMethodItem {
  id: number;
  name: string;
  labelEn: string | null;
  labelAr: string | null;
  type: string | null;
  gateway: string | null;
  gatewayConfig: string | null;
  iconUrl: string | null;
  instructionsEn: string | null;
  instructionsAr: string | null;
  isActive: boolean;
  sortOrder: number;
  createdAt: string;
  updatedAt: string | null;
}

function normalizeType(type: string | null): PaymentMethodType | null {
  if (type === 'offline' || type === 'online') return type;
  return null;
}

function toListItem(raw: BackendPaymentMethodItem): PaymentMethodListItem {
  return {
    id: raw.id,
    name: raw.name,
    labelEn: raw.labelEn,
    labelAr: raw.labelAr,
    type: normalizeType(raw.type),
    gateway: raw.gateway,
    iconUrl: raw.iconUrl,
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
  };
}

function toDto(raw: BackendPaymentMethodItem): PaymentMethodDto {
  return {
    id: raw.id,
    name: raw.name,
    labelEn: raw.labelEn,
    labelAr: raw.labelAr,
    type: normalizeType(raw.type),
    gateway: raw.gateway,
    gatewayConfig: raw.gatewayConfig,
    iconUrl: raw.iconUrl,
    instructionsEn: raw.instructionsEn,
    instructionsAr: raw.instructionsAr,
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
  };
}

export interface CreatePaymentMethodPayload {
  name: string;
  labelEn?: string;
  labelAr?: string;
  type?: PaymentMethodType;
  gateway?: string;
  gatewayConfig?: string;
  iconUrl?: string;
  instructionsEn?: string;
  instructionsAr?: string;
  status?: 'active' | 'inactive';
  sortOrder?: number;
}

export interface UpdatePaymentMethodPayload {
  name: string;
  labelEn?: string;
  labelAr?: string;
  type?: PaymentMethodType;
  gateway?: string;
  gatewayConfig?: string;
  iconUrl?: string;
  instructionsEn?: string;
  instructionsAr?: string;
  status?: 'active' | 'inactive';
  sortOrder?: number;
}

export interface ChangePaymentMethodStatusPayload {
  isActive: boolean;
}

export interface ChangePaymentMethodStatusResult {
  id: number;
  status: 'active' | 'inactive';
  updatedAt: string | null;
}

function buildBackendPayload(payload: CreatePaymentMethodPayload | UpdatePaymentMethodPayload) {
  return {
    name: payload.name,
    labelEn: payload.labelEn ?? null,
    labelAr: payload.labelAr ?? null,
    type: payload.type ?? null,
    gateway: payload.gateway ?? null,
    gatewayConfig: payload.gatewayConfig ?? null,
    iconUrl: payload.iconUrl ?? null,
    instructionsEn: payload.instructionsEn ?? null,
    instructionsAr: payload.instructionsAr ?? null,
    isActive: payload.status !== 'inactive',
    sortOrder: payload.sortOrder ?? 0,
  };
}

export async function createPaymentMethod(
  payload: CreatePaymentMethodPayload,
): Promise<PaymentMethodDto> {
  const { data } = await apiClient.post<ApiResponse<BackendPaymentMethodItem>>(
    '/payment-methods',
    buildBackendPayload(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to create payment method');
  return toDto(data.data!);
}

export async function updatePaymentMethod(
  id: number,
  payload: UpdatePaymentMethodPayload,
): Promise<PaymentMethodDto> {
  const { data } = await apiClient.put<ApiResponse<BackendPaymentMethodItem>>(
    `/payment-methods/${id}`,
    buildBackendPayload(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update payment method');
  return toDto(data.data!);
}

export async function deletePaymentMethod(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/payment-methods/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete payment method');
}

export async function changePaymentMethodStatus(
  id: number,
  payload: ChangePaymentMethodStatusPayload,
): Promise<ChangePaymentMethodStatusResult> {
  const { data } = await apiClient.put<
    ApiResponse<{
      id: number;
      isActive: boolean;
      updatedAt: string | null;
    }>
  >(`/payment-methods/${id}/status`, {
    isActive: payload.isActive,
  });

  if (!data.success) {
    throw new Error(data.message ?? 'Failed to change payment method status');
  }

  const raw = data.data!;
  return {
    id: raw.id,
    status: raw.isActive ? 'active' : 'inactive',
    updatedAt: raw.updatedAt,
  };
}

export async function getPaymentMethodById(id: number): Promise<PaymentMethodDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendPaymentMethodItem>>(
    `/payment-methods/${id}`,
  );
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function getPaymentMethods(
  params: PaymentMethodQueryParams = {},
): Promise<PaginatedResponse<PaymentMethodListItem>> {
  const { page = 1, limit = 10, search, type, status, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendPaymentMethodItem[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >('/payment-methods', {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(type && { type }),
      ...(status !== undefined && { isActive: status === 'active' }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch payment methods');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
