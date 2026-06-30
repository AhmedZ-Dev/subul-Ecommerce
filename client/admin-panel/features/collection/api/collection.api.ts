// features/collection/api/collection.api.ts
import apiClient from '@/lib/api-client';
import type { CollectionDto, CollectionListItem, CollectionQueryParams } from '../types';
import type { ApiResponse, PaginatedResponse } from '@/types/api';

interface BackendCollectionItem {
  id: number;
  nameEn: string;
  nameAr: string | null;
  slug: string;
  descriptionEn: string | null;
  descriptionAr: string | null;
  imageUrl: string | null;
  bannerUrl: string | null;
  collectionType: string;
  isActive: boolean;
  sortOrder: number;
  metaTitle: string | null;
  metaDescription: string | null;
  createdAt: string;
  updatedAt: string | null;
}

function toListItem(raw: BackendCollectionItem): CollectionListItem {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    collectionType: raw.collectionType as 'manual' | 'smart',
    sortOrder: raw.sortOrder,
    status: raw.isActive ? 'active' : 'inactive',
    createdAt: raw.createdAt,
  };
}

function toDto(raw: BackendCollectionItem): CollectionDto {
  return {
    id: raw.id,
    nameEn: raw.nameEn,
    nameAr: raw.nameAr,
    slug: raw.slug,
    descriptionEn: raw.descriptionEn,
    descriptionAr: raw.descriptionAr,
    imageUrl: raw.imageUrl,
    bannerUrl: raw.bannerUrl,
    collectionType: raw.collectionType as 'manual' | 'smart',
    status: raw.isActive ? 'active' : 'inactive',
    sortOrder: raw.sortOrder,
    metaTitle: raw.metaTitle,
    metaDescription: raw.metaDescription,
    createdAt: raw.createdAt,
    updatedAt: raw.updatedAt,
  };
}

export interface CreateCollectionPayload {
  nameEn: string;
  nameAr?: string;
  slug?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  collectionType: 'manual' | 'smart';
  status?: 'active' | 'inactive';
  sortOrder?: number;
  metaTitle?: string;
  metaDescription?: string;
}

export interface UpdateCollectionPayload {
  nameEn?: string;
  nameAr?: string;
  slug?: string;
  descriptionEn?: string;
  descriptionAr?: string;
  collectionType?: 'manual' | 'smart';
  status?: 'active' | 'inactive';
  sortOrder?: number;
  metaTitle?: string;
  metaDescription?: string;
}

export interface ChangeCollectionStatusPayload {
  isActive: boolean;
}

export interface ChangeCollectionStatusResult {
  id: number;
  status: 'active' | 'inactive';
  updatedAt: string | null;
}

export async function createCollection(payload: CreateCollectionPayload): Promise<CollectionDto> {
  const { data } = await apiClient.post<ApiResponse<BackendCollectionItem>>('/collections', {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr,
    slug: payload.slug,
    descriptionEn: payload.descriptionEn,
    descriptionAr: payload.descriptionAr,
    collectionType: payload.collectionType,
    isActive: payload.status !== 'inactive',
    sortOrder: payload.sortOrder ?? 0,
    metaTitle: payload.metaTitle,
    metaDescription: payload.metaDescription,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to create collection');
  return toDto(data.data!);
}

export async function updateCollection(
  id: number,
  payload: UpdateCollectionPayload,
): Promise<CollectionDto> {
  const { data } = await apiClient.put<ApiResponse<BackendCollectionItem>>(`/collections/${id}`, {
    nameEn: payload.nameEn,
    nameAr: payload.nameAr ?? null,
    slug: payload.slug ?? null,
    descriptionEn: payload.descriptionEn ?? null,
    descriptionAr: payload.descriptionAr ?? null,
    collectionType: payload.collectionType,
    isActive: payload.status !== 'inactive',
    sortOrder: payload.sortOrder ?? 0,
    metaTitle: payload.metaTitle ?? null,
    metaDescription: payload.metaDescription ?? null,
  });
  if (!data.success) throw new Error(data.message ?? 'Failed to update collection');
  return toDto(data.data!);
}

export async function deleteCollection(id: number): Promise<void> {
  const { data } = await apiClient.delete<ApiResponse<null>>(`/collections/${id}`);
  if (!data.success) throw new Error(data.message ?? 'Failed to delete collection');
}

export async function changeCollectionStatus(
  id: number,
  payload: ChangeCollectionStatusPayload,
): Promise<ChangeCollectionStatusResult> {
  const { data } = await apiClient.put<
    ApiResponse<{
      id: number;
      isActive: boolean;
      updatedAt: string | null;
    }>
  >(`/collections/${id}/status`, {
    isActive: payload.isActive,
  });

  if (!data.success) {
    throw new Error(data.message ?? 'Failed to change collection status');
  }

  const raw = data.data!;
  return {
    id: raw.id,
    status: raw.isActive ? 'active' : 'inactive',
    updatedAt: raw.updatedAt,
  };
}

export async function getCollectionById(id: number): Promise<CollectionDto | null> {
  const { data } = await apiClient.get<ApiResponse<BackendCollectionItem>>(`/collections/${id}`);
  if (!data.success) return null;
  return data.data ? toDto(data.data) : null;
}

export async function getCollections(
  params: CollectionQueryParams = {},
): Promise<PaginatedResponse<CollectionListItem>> {
  const { page = 1, limit = 10, search, status, type, sortBy, sortOrder } = params;

  const { data } = await apiClient.get<
    ApiResponse<{ items: BackendCollectionItem[]; total: number; page: number; limit: number; totalPages: number }>
  >('/collections', {
    params: {
      page,
      limit,
      ...(search && { search }),
      ...(status !== undefined && { isActive: status === 'active' }),
      ...(type && { collectionType: type }),
      ...(sortBy && { sortBy }),
      ...(sortOrder && { sortOrder }),
    },
  });

  if (!data.success) throw new Error(data.message ?? 'Failed to fetch collections');

  const raw = data.data!;
  return {
    items: raw.items.map(toListItem),
    total: raw.total,
    page: raw.page,
    limit: raw.limit,
    totalPages: raw.totalPages,
  };
}
