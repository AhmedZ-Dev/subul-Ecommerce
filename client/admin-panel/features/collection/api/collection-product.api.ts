import apiClient from '@/lib/api-client';
import type { CollectionProductInfo, CollectionProductQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendCollectionProductResponse {
  id: number;
  collectionId: number;
  productId: number;
  sortOrder: number;
  createdAt: string;
  product: {
    nameEn: string;
    nameAr: string | null;
    slug: string;
    price: number;
  };
}

function toInfo(raw: BackendCollectionProductResponse): CollectionProductInfo {
  return {
    id: raw.id,
    collectionId: raw.collectionId,
    productId: raw.productId,
    sortOrder: raw.sortOrder,
    createdAt: raw.createdAt,
    product: {
      nameEn: raw.product.nameEn,
      nameAr: raw.product.nameAr,
      slug: raw.product.slug,
      price: raw.product.price,
    },
  };
}

export interface AddCollectionProductPayload {
  productId: number;
  sortOrder?: number;
}

export interface UpdateCollectionProductPayload {
  sortOrder: number;
}

export async function getCollectionProducts(
  collectionId: number,
  params: CollectionProductQueryParams = {},
): Promise<PaginatedResponse<CollectionProductInfo>> {
  const { page = 1, limit = 10, search, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{
      items: BackendCollectionProductResponse[];
      total: number;
      page: number;
      limit: number;
      totalPages: number;
    }>
  >(`/collections/${collectionId}/products`, {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch collection products');

  const raw = data.data!;
  return {
    items: raw.items.map(toInfo),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}

export async function getCollectionProductById(
  collectionId: number,
  id: number,
): Promise<CollectionProductInfo | null> {
  const { data } = await apiClient.get<ApiResponse<BackendCollectionProductResponse>>(
    `/collections/${collectionId}/products/${id}`,
  );
  if (!data.success) return null;
  return data.data ? toInfo(data.data) : null;
}

export async function addCollectionProduct(
  collectionId: number,
  payload: AddCollectionProductPayload,
): Promise<CollectionProductInfo> {
  const { data } = await apiClient.post<ApiResponse<BackendCollectionProductResponse>>(
    `/collections/${collectionId}/products`,
    {
      productId: payload.productId,
      sortOrder: payload.sortOrder ?? 0,
    },
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to add product to collection');
  return toInfo(data.data!);
}

export async function updateCollectionProduct(
  collectionId: number,
  id: number,
  payload: UpdateCollectionProductPayload,
): Promise<CollectionProductInfo> {
  const { data } = await apiClient.put<ApiResponse<BackendCollectionProductResponse>>(
    `/collections/${collectionId}/products/${id}`,
    payload,
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to update collection product');
  return toInfo(data.data!);
}

export async function deleteCollectionProduct(collectionId: number, id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<boolean>>(
    `/collections/${collectionId}/products/${id}`,
  );
  if (!data.success) throw new Error(data.message ?? 'Failed to remove product from collection');
}
