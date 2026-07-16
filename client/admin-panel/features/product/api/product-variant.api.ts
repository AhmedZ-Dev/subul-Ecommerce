import apiClient from '@/lib/api-client';
import type { ProductVariantInfo, ProductVariantQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendVariantResponse {
  id: number;
  productId: number;
  title: string | null;
  sku: string | null;
  barcode: string | null;
  price: number | null;
  compareAtPrice: number | null;
  costPrice: number | null;
  stockQuantity: number;
  weight: number | null;
  isActive: boolean;
  sortOrder: number;
  createdAt: string;
  updatedAt: string | null;
}

function toVariantInfo(raw: BackendVariantResponse): ProductVariantInfo {
  return {
    id: raw.id,
    title: raw.title,
    sku: raw.sku,
    barcode: raw.barcode,
    price: raw.price,
    compareAtPrice: raw.compareAtPrice,
    costPrice: raw.costPrice,
    stockQuantity: raw.stockQuantity,
    weight: raw.weight,
    isActive: raw.isActive,
    sortOrder: raw.sortOrder,
  };
}

export interface CreateProductVariantPayload {
  title?: string;
  sku?: string;
  barcode?: string;
  price?: number | null;
  compareAtPrice?: number | null;
  costPrice?: number | null;
  stockQuantity?: number;
  weight?: number | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface UpdateProductVariantPayload {
  title?: string | null;
  sku?: string | null;
  barcode?: string | null;
  price?: number | null;
  compareAtPrice?: number | null;
  costPrice?: number | null;
  stockQuantity: number;
  weight?: number | null;
  isActive: boolean;
  sortOrder: number;
}

function buildCreateBody(payload: CreateProductVariantPayload) {
  return {
    title: payload.title ?? null,
    sku: payload.sku ?? null,
    barcode: payload.barcode ?? null,
    price: payload.price ?? null,
    compareAtPrice: payload.compareAtPrice ?? null,
    costPrice: payload.costPrice ?? null,
    stockQuantity: payload.stockQuantity ?? 0,
    weight: payload.weight ?? null,
    isActive: payload.isActive ?? true,
    sortOrder: payload.sortOrder ?? 0,
  };
}

function buildUpdateBody(payload: UpdateProductVariantPayload) {
  return {
    title: payload.title ?? null,
    sku: payload.sku ?? null,
    barcode: payload.barcode ?? null,
    price: payload.price ?? null,
    compareAtPrice: payload.compareAtPrice ?? null,
    costPrice: payload.costPrice ?? null,
    stockQuantity: payload.stockQuantity,
    weight: payload.weight ?? null,
    isActive: payload.isActive,
    sortOrder: payload.sortOrder,
  };
}

export async function createProductVariant(
  productId: number,
  payload: CreateProductVariantPayload,
): Promise<ProductVariantInfo> {
  const { data } = await apiClient.post<ApiResponse<BackendVariantResponse>>(
    `/products/${productId}/variants`,
    buildCreateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to create product variant');
  return toVariantInfo(data.data!);
}

export async function updateProductVariant(
  productId: number,
  variantId: number,
  payload: UpdateProductVariantPayload,
): Promise<ProductVariantInfo> {
  const { data } = await apiClient.put<ApiResponse<BackendVariantResponse>>(
    `/products/${productId}/variants/${variantId}`,
    buildUpdateBody(payload),
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update product variant');
  return toVariantInfo(data.data!);
}

export async function deleteProductVariant(productId: number, variantId: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<boolean>>(
    `/products/${productId}/variants/${variantId}`,
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to delete product variant');
}

export async function getProductVariantById(
  productId: number,
  variantId: number,
): Promise<ProductVariantInfo | null> {
  const { data } = await apiClient.get<ApiResponse<BackendVariantResponse>>(
    `/products/${productId}/variants/${variantId}`,
  );
  if (!data.success) return null;
  return data.data ? toVariantInfo(data.data) : null;
}

export async function getProductVariants(
  productId: number,
  params: ProductVariantQueryParams = {},
): Promise<PaginatedResponse<ProductVariantInfo>> {
  const { page = 1, limit = 50, search, isActive, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendVariantResponse[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >(`/products/${productId}/variants`, {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(isActive !== undefined && { isActive }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch product variants');

  const raw = data.data!;
  return {
    items: raw.items.map(toVariantInfo),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
