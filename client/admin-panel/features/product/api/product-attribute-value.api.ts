import apiClient from '@/lib/api-client';
import type { ProductAttributeValueInfo, ProductAttributeValueQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendAttributeInfo {
  nameEn: string;
  nameAr: string | null;
  unit: string | null;
  inputType: string;
}

interface BackendAttributeValueResponse {
  id: number;
  productId: number;
  attributeId: number;
  valueText: string | null;
  valueNumber: number | null;
  valueBoolean: boolean | null;
  createdAt: string;
  attribute: BackendAttributeInfo;
}

function toAttributeValueInfo(raw: BackendAttributeValueResponse): ProductAttributeValueInfo {
  return {
    id: raw.id,
    attributeId: raw.attributeId,
    valueText: raw.valueText,
    valueNumber: raw.valueNumber,
    valueBoolean: raw.valueBoolean,
    attribute: {
      nameEn: raw.attribute.nameEn,
      nameAr: raw.attribute.nameAr,
      unit: raw.attribute.unit,
      inputType: raw.attribute.inputType,
      sortOrder: 0,
    },
  };
}

export interface CreateProductAttributeValuePayload {
  attributeId: number;
  valueText?: string;
  valueNumber?: number | null;
  valueBoolean?: boolean | null;
}

export interface UpdateProductAttributeValuePayload {
  attributeId: number;
  valueText?: string | null;
  valueNumber?: number | null;
  valueBoolean?: boolean | null;
}

function buildCreateBody(payload: CreateProductAttributeValuePayload) {
  return {
    attributeId: payload.attributeId,
    valueText: payload.valueText ?? null,
    valueNumber: payload.valueNumber ?? null,
    valueBoolean: payload.valueBoolean ?? null,
  };
}

function buildUpdateBody(payload: UpdateProductAttributeValuePayload) {
  return {
    attributeId: payload.attributeId,
    valueText: payload.valueText ?? null,
    valueNumber: payload.valueNumber ?? null,
    valueBoolean: payload.valueBoolean ?? null,
  };
}

export async function createProductAttributeValue(
  productId: number,
  payload: CreateProductAttributeValuePayload,
): Promise<ProductAttributeValueInfo> {
  const { data } = await apiClient.post<ApiResponse<BackendAttributeValueResponse>>(
    `/products/${productId}/attribute-values`,
    buildCreateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to create product attribute value');
  return toAttributeValueInfo(data.data!);
}

export async function updateProductAttributeValue(
  productId: number,
  valueId: number,
  payload: UpdateProductAttributeValuePayload,
): Promise<ProductAttributeValueInfo> {
  const { data } = await apiClient.put<ApiResponse<BackendAttributeValueResponse>>(
    `/products/${productId}/attribute-values/${valueId}`,
    buildUpdateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update product attribute value');
  return toAttributeValueInfo(data.data!);
}

export async function deleteProductAttributeValue(
  productId: number,
  valueId: number,
): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<boolean>>(
    `/products/${productId}/attribute-values/${valueId}`,
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to delete product attribute value');
}

export async function getProductAttributeValueById(
  productId: number,
  valueId: number,
): Promise<ProductAttributeValueInfo | null> {
  const { data } = await apiClient.get<ApiResponse<BackendAttributeValueResponse>>(
    `/products/${productId}/attribute-values/${valueId}`,
  );
  if (!data.success) return null;
  return data.data ? toAttributeValueInfo(data.data) : null;
}

export async function getProductAttributeValues(
  productId: number,
  params: ProductAttributeValueQueryParams = {},
): Promise<PaginatedResponse<ProductAttributeValueInfo>> {
  const { page = 1, limit = 50, search, attributeId, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendAttributeValueResponse[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >(`/products/${productId}/attribute-values`, {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(attributeId != null && { attributeId }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch product attribute values');

  const raw = data.data!;
  return {
    items: raw.items.map(toAttributeValueInfo),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
