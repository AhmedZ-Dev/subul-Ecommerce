import apiClient, { postFormData } from '@/lib/api-client';
import type { ProductImageInfo, ProductImageQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendImageResponse {
  id: number;
  productId: number;
  variantId: number | null;
  imageUrl: string;
  altText: string | null;
  sortOrder: number;
  isPrimary: boolean;
  createdAt: string;
}

function toImageInfo(raw: BackendImageResponse): ProductImageInfo {
  return {
    id: raw.id,
    productId: raw.productId,
    variantId: raw.variantId,
    imageUrl: raw.imageUrl,
    altText: raw.altText,
    sortOrder: raw.sortOrder,
    isPrimary: raw.isPrimary,
    createdAt: raw.createdAt,
  };
}

export interface CreateProductImagePayload {
  file: File;
  variantId?: number | null;
  altText?: string;
  sortOrder?: number;
  isPrimary?: boolean;
}

export interface UpdateProductImagePayload {
  variantId?: number | null;
  altText?: string | null;
  sortOrder: number;
  isPrimary: boolean;
}

export async function createProductImage(
  productId: number,
  payload: CreateProductImagePayload,
): Promise<ProductImageInfo> {
  const formData = new FormData();
  formData.append('Image', payload.file);
  if (payload.variantId != null) formData.append('VariantId', String(payload.variantId));
  if (payload.altText) formData.append('AltText', payload.altText);
  formData.append('SortOrder', String(payload.sortOrder ?? 0));
  formData.append('IsPrimary', String(payload.isPrimary ?? false));

  const raw = await postFormData<BackendImageResponse>(
    `/products/${productId}/images`,
    formData,
  );
  return toImageInfo(raw);
}

export async function updateProductImage(
  productId: number,
  imageId: number,
  payload: UpdateProductImagePayload,
): Promise<ProductImageInfo> {
  const { data } = await apiClient.put<ApiResponse<BackendImageResponse>>(
    `/products/${productId}/images/${imageId}`,
    {
      variantId: payload.variantId ?? null,
      altText: payload.altText ?? null,
      sortOrder: payload.sortOrder,
      isPrimary: payload.isPrimary,
    },
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update product image');
  return toImageInfo(data.data!);
}

export async function deleteProductImage(productId: number, imageId: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<boolean>>(
    `/products/${productId}/images/${imageId}`,
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to delete product image');
}

export async function getProductImageById(
  productId: number,
  imageId: number,
): Promise<ProductImageInfo | null> {
  const { data } = await apiClient.get<ApiResponse<BackendImageResponse>>(
    `/products/${productId}/images/${imageId}`,
  );
  if (!data.success) return null;
  return data.data ? toImageInfo(data.data) : null;
}

export async function getProductImages(
  productId: number,
  params: ProductImageQueryParams = {},
): Promise<PaginatedResponse<ProductImageInfo>> {
  const { page = 1, limit = 50, variantId, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendImageResponse[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >(`/products/${productId}/images`, {
    params: {
      page,
      limit,
      ...(variantId != null && { variantId }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch product images');

  const raw = data.data!;
  return {
    items: raw.items.map(toImageInfo),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
